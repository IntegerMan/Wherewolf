namespace MattEland.Wherewolf.Phases;

public abstract class GamePhase
{
    public abstract void Run(GameState newState, Action<GameState> callback);
    
    public abstract double Order { get; }
    public abstract string Name { get; }
    public GamePhase? PriorPhase { get; internal set; }

    public abstract IEnumerable<GameState> BuildPossibleStates(GameState priorState);

    public override string ToString() => Name;

    public virtual bool AutoAdvance { get; } = false;
} 