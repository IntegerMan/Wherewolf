namespace MattEland.Wherewolf.Events.Social;

public abstract class SocialEvent
{
    public abstract string Description { get; }

    public abstract bool IsPossibleInGameState(GameState state);

    public abstract bool IsClaimValidFor(GameState gameState);
    public override string ToString() => Description;
}