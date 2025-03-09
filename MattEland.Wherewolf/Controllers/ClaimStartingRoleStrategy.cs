using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimStartingRoleStrategy : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState) 
        => gameState.GetStartRole(player);
    
    public SpecificRoleClaim GetSpecificRoleClaim(Player player, GameState gameState, SpecificRoleClaim[] possibleClaims,
        GameRole initialClaim)
    {
        return possibleClaims.First(claim => claim.Player == player && claim.Role == initialClaim);
    }
}