namespace MattEland.Wherewolf.Events;

public abstract class GameEvent
{
    public abstract bool IsObservedBy(Player player);
    public abstract string Description { get; }
}