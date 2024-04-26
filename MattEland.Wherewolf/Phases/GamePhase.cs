namespace MattEland.Wherewolf.Phases;

public abstract class GamePhase
{
    public abstract GameState Run(GameState newState);
    
    public abstract double Order { get; }
    public abstract string Name { get; }

    public abstract IEnumerable<GameState> BuildPossibleStates(GameState priorState);
} 