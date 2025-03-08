using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public abstract class SocialEvent(Player player) : IGameEvent
{
    public Player Player => player;
    public abstract string Description { get; }
    public virtual Team? AssociatedTeam => null;

    public abstract bool IsClaimValidFor(GameState state);
    public override string ToString() => Description;
}