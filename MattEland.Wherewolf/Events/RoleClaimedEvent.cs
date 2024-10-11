using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public class RoleClaimedEvent : GameEvent
{
    public RoleClaimedEvent(Player player, GameRole role)
    {
        this.Player = player;
        this.ClaimedRole = role;
    }

    public Player Player { get; }
    public GameRole ClaimedRole { get;  }
    public override bool IsObservedBy(Player player) => true;

    public override string Description => $"{Player.Name} claimed their role was {ClaimedRole}";
    public override bool IsPossibleInGameState(GameState state) => state.Roles.Contains(ClaimedRole);
}