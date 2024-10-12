using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimStartingRoleStrategy : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState) 
        => gameState.GetStartRole(player);
}