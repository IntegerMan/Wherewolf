using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf;

public class GamePermutation
{
    public GameState State { get; }
    public double Support { get; }
    
    public GamePermutation(GameState state, double support)
    {
        State = state;
        Support = support;
    }

    public bool IsPossibleGivenEvents(IEnumerable<GameEvent> events) 
        => events.All(e => e.IsPossibleInGameState(State));

    public IEnumerable<GamePermutation> ExtrapolateEndPermutations()
    {
        if (State.IsGameOver || State.CurrentPhase is null)
        {
            throw new InvalidOperationException("Cannot extrapolate from end of game state");
        }
        
        List<GameState> possibleNextStates = State.CurrentPhase!.BuildPossibleStates(State).ToList();
        foreach (var permutation in possibleNextStates)
        {
            GamePermutation next = new GamePermutation(permutation, Support / possibleNextStates.Count);

            if (next.State.IsGameOver)
            {
                yield return next;
            }
            else
            {
                foreach (var result in next.ExtrapolateEndPermutations())
                {
                    yield return result;
                }
            }
        }
    }

    public override string ToString() 
        => $"{State} - Support: {Support}";
}