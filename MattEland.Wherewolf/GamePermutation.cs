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

    public override string ToString() 
        => $"{State} - Support: {Support}";
}