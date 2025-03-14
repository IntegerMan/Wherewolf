namespace MattEland.Wherewolf.Phases;

public abstract class GamePhase
{
    public abstract void Run(PhaseContext context);
    
    public abstract double Order { get; }
    public abstract string Name { get; }
    public GamePhase? PriorPhase { get; internal set; }

    public abstract IEnumerable<GameState> BuildPossibleStates(GameState priorState);

    public override string ToString() => Name;
} 