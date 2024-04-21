using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf;

public record PlayerState(Player Player, GameState GameState)
{
    private readonly List<GameEvent> _events = new();
    private PlayerProbabilities? _probabilities;
    public IEnumerable<GameEvent> ObservedEvents => _events.AsReadOnly();

    public void AddEvents(IEnumerable<GameEvent> gameEvents)
    {
        _events.AddRange(gameEvents);
    }

    public void AddEvent(GameEvent gameEvent)
    {
        _events.Add(gameEvent);
    }
    
    public PlayerProbabilities Probabilities
    {
        get {
            return _probabilities ??= CalculateProbabilities();
        }
    }

    private PlayerProbabilities CalculateProbabilities()
    {
        PlayerProbabilities probabilities = new(this);

        foreach (var slot in GameState.AllSlots)
        {
            // Start with all permutations
            IEnumerable<GamePermutation> permutations = GameState.Setup.Permutations;
        
            // Filter down to a set of permutations where the observed events are possible
            permutations = permutations.Where(p => p.IsPossibleGivenPlayerState(this));
            int totalSupport = permutations.Sum(p => p.Support);
        
            foreach (var role in GameState.Roles.DistinctBy(r => r.Name))
            {
                // Figure out the number of possible worlds where the player had the role at the start
                int roleSupport = permutations.Where(p => p.State.GetSlot(slot.Name).StartRole.Name == role.Name).Sum(p => p.Support);

                probabilities.RegisterSlotRoleProbabilities(slot, role.Name, roleSupport, totalSupport);
            }            
        }
        
        return probabilities;
    }
}