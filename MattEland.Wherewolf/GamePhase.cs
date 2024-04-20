namespace MattEland.Wherewolf;

public abstract class GamePhase
{
    public abstract GameState Run(GameState newState);
    
    public abstract double Order { get; }
} 