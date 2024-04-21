namespace MattEland.Wherewolf;

public class GamePermutation
{
    public GameState State { get; }
    public int Support { get; }
    
    public GamePermutation(GameState state, int support)
    {
        State = state;
        Support = support;
    }

    public bool IsPossibleGivenPlayerState(PlayerState playerState) 
        => playerState.ObservedEvents.All(e => e.IsPossibleInGameState(State));
}