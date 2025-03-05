using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public abstract class SocialEvent : IGameEvent
{
    public abstract string Description { get; }
    public virtual Team? AssociatedTeam => null;

    public abstract bool IsPossibleInGameState(GameState state);

    public abstract bool IsClaimValidFor(GameState gameState);
    public override string ToString() => Description;
}