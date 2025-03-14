using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf;

public class GameManager
{
    private readonly GameSetup _setup;
    public List<SocialEvent> Claims { get; } = [];

    public GameManager(GameSetup setup)
    {
        _setup = setup;
        CurrentState = setup.StartGame();
    }
    
    public GameState CurrentState { get; private set; }

    public void RunToEndOfNight()
    {
        CurrentState.RunToEndOfNight(state => CurrentState = state);
    }

    public void RunToEnd()
    {
        CurrentState.RunToEnd(state => CurrentState = state);
    }
    
    public IEnumerable<IGameEvent> EventsForPlayer(Player? player = null)
    {
        foreach (var evt in CurrentState.Events)
        {
            // If it's a standard event and the player saw it or if we're omniscient, show it
            if (player is null || evt.IsObservedBy(player) || IsGameOver)
                yield return evt;

            // At this point we can marry in our social claims observed
            if (evt is MakeSocialClaimsNowEvent)
            {
                foreach (var social in Claims)
                {
                    yield return social;
                }
            }
        }
    }

    public bool IsGameOver => CurrentState.IsGameOver;
    
    
    public PlayerProbabilities CalculateProbabilities(Player player)
    {
        PlayerProbabilities probabilities = new();

        // Start with all permutations
        GameState[] validPermutations = VotingHelper.GetPossibleGameStatesForPlayer(player, CurrentState).ToArray();

        double startPopulation = validPermutations.Sum(p => p.Support);
        
        StartRoleClaimedEvent[] claimedEvents = Claims.OfType<StartRoleClaimedEvent>().ToArray();

        // Calculate starting role probabilities
        foreach (var slot in CurrentState.AllSlots)
        {
            foreach (var role in _setup.Roles.Distinct())
            {
                // Figure out the number of possible worlds where the slot had the role at the start
                double startRoleSupport = validPermutations.Where(p => p.Root[slot.Name].Role == role)
                    .Sum(p => p.Support);

                IEnumerable<Player> startSupport = claimedEvents
                    .Where(e => e.ClaimedRole == role && e.Player == slot.Player && e.IsClaimValidFor(CurrentState))
                    .Select(e => e.Player)
                    .Where(e => e != player)
                    .Distinct();
                
                probabilities.RegisterStartRoleProbabilities(slot, role, startRoleSupport, startPopulation, startSupport);
                
                // Figure out the number of possible worlds where the slot currently has the role
                IEnumerable<GameState> endGameStates = validPermutations.Where(p => p[slot.Name].Role == role);
                double currentRoleSupport = endGameStates.Sum(p => p.Support);
                
                probabilities.RegisterCurrentRoleProbabilities(slot, role, currentRoleSupport, startPopulation, []);
            }
        }
        
        return probabilities;
    }
}