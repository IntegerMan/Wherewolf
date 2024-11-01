using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public class StartRoleClaimedEvent : GameEvent
{
    public StartRoleClaimedEvent(Player player, GameRole role)
    {
        this.Player = player;
        this.ClaimedRole = role;
    }

    public Player Player { get; }
    public GameRole ClaimedRole { get;  }
    public override bool IsObservedBy(Player player) => true;

    public override string Description => $"{Player.Name} claimed their starting role was {ClaimedRole}";
    public override bool IsPossibleInGameState(GameState state) =>
        state.Claims.Any(e => e.Player == Player && e.ClaimedRole == ClaimedRole);

    public bool IsClaimValidFor(GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(Player);
        return startRole == ClaimedRole;
    }
}