using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public class RobberRobbedClaim(Player player, Player target, GameRole stolenRole) : SpecificRoleClaim(player, GameRole.Robber)
{
    public override string Description => $"{Player} claimed they were the robber and stole {StolenRole} from {Target}";
    
    public GameRole StolenRole => stolenRole;
    public Player Target => target;
    
    public override bool IsClaimValidFor(GameState state) 
        => state.Events.OfType<RobbedPlayerEvent>()
            .Any(e => e.PlayerName == Player.Name && e.TargetName == Target.Name && e.NewRole == StolenRole);

    public override bool IsCombinatoriallyPossible(GameState state)
        => state.GetRoleCount(GameRole.Robber) >= 1
        && (StolenRole != GameRole.Robber
            ? state.GetRoleCount(StolenRole) >= 1
            : state.GetRoleCount(GameRole.Robber) >= 2);
}