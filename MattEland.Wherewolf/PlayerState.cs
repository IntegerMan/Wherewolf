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
        PlayerProbabilities probabilities = new();

        // Start with all permutations
        List<GamePermutation> startingPermutations = GameState.Setup.Permutations.Where(p => p.IsPossibleGivenPlayerState(this)).ToList();
        
        double startPopulation = startingPermutations.Sum(p => p.Support);
        
        // Calculate starting role probabilities
        foreach (var slot in GameState.AllSlots)
        {
            foreach (var role in GameState.Roles.DistinctBy(r => r.Name))
            {
                // Figure out the number of possible worlds where the slot had the role at the start
                double roleSupport = startingPermutations.Where(p => p.State.GetSlot(slot.Name).StartRole.Name == role.Name)
                                              .Sum(p => p.Support);

                probabilities.RegisterSlotRoleProbabilities(slot, isStarting: true, role.Name, roleSupport, startPopulation);
            }
        }
        
        // Given our valid start permutations, simulate all possible game states that could arise from the next night phase
        List<GamePermutation> endPermutations = new(); //startingPermutations.SelectMany(p => p.ExtrapolateEndPermutations()).ToList();

        foreach (var startPermutation in startingPermutations)
        {
            List<GamePermutation> childPermutations = startPermutation.ExtrapolateEndPermutations().ToList();

            if (!childPermutations.Any())
                throw new InvalidOperationException($"{startPermutation} yielded no end permutations on phase {startPermutation.State.CurrentPhase.GetType().Name}");
            
            endPermutations.AddRange(childPermutations);
        }
        
        // Filter down to a set of permutations where the observed events are possible
        endPermutations = endPermutations.Where(p => p.IsPossibleGivenPlayerState(this)).ToList();

        double endPopulation = endPermutations.Sum(p => p.Support);

        // Calculate ending role probabilities
        foreach (var slot in GameState.AllSlots)
        {
            foreach (var role in GameState.Roles.DistinctBy(r => r.Name))
            {
                // Figure out the number of possible worlds where the slot had the role at the end
                double roleSupport = endPermutations.Where(p => p.State.GetSlot(slot.Name).CurrentRole.Name == role.Name)
                    .Sum(p => p.Support);

                probabilities.RegisterSlotRoleProbabilities(slot, isStarting: false, role.Name, roleSupport, endPopulation);
            }
        }
        
        return probabilities;
    }
}