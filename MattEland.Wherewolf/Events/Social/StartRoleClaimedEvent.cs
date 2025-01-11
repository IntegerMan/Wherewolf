using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public class StartRoleClaimedEvent : SocialEvent
{
    public StartRoleClaimedEvent(Player player, GameRole role)
    {
        this.Player = player;
        this.ClaimedRole = role;
    }

    public Player Player { get; }
    public GameRole ClaimedRole { get;  }

    public override string Description => $"{Player.Name} claimed their starting role was {ClaimedRole}";
    public override bool IsPossibleInGameState(GameState state) =>
        state.Claims
            .OfType<StartRoleClaimedEvent>()
            .Any(e => e.Player == Player && e.ClaimedRole == ClaimedRole);

    public override bool IsClaimValidFor(GameState gameState) 
        => gameState.GetStartRole(Player) == ClaimedRole;
}