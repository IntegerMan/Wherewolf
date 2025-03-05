using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public abstract class GameEvent
{
    public abstract bool IsObservedBy(Player player);
    public abstract string Description { get; }
    public virtual Team? AssociatedTeam => null;

    public abstract bool IsPossibleInGameState(GameState state);

    public override string ToString() => Description;
}