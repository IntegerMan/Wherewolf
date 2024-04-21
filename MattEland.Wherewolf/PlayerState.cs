using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf;

public record PlayerState(Player Player, GameSetup GameSetup)
{
    private readonly List<GameEvent> _events = new();
    public IEnumerable<GameEvent> ObservedEvents => _events.AsReadOnly();

    public void AddEvents(IEnumerable<GameEvent> gameEvents)
    {
        _events.AddRange(gameEvents);
    }

    public void AddEvent(GameEvent gameEvent)
    {
        _events.Add(gameEvent);
    }

    public IEnumerable<CardProbability> CalculateStartingCardProbabilities(Player player)
    {
        // Start with all permutations
        IEnumerable<GamePermutation> permutations = GameSetup.Permutations;
        
        // Filter down to a set of permutations where the observed events are possible
        permutations = permutations.Where(p => ObservedEvents.All(e => e.IsPossibleInGameState(p.State)));
        int totalSupport = permutations.Sum(p => p.Support);
        
        foreach (var role in GameSetup.Roles.DistinctBy(r => r.Name))
        {
            // Figure out the number of possible worlds where the player had the role at the start
            int roleSupport = permutations.Where(p => p.State.GetPlayerSlot(player).StartRole.Name == role.Name).Sum(p => p.Support);

            // Only yield a role if we think it could be possible
            if (roleSupport > 0)
            {
                yield return new CardProbability(role, (double)roleSupport / totalSupport); // Calculate probability based on role support and overall scenario support
            }
        }
    }
}