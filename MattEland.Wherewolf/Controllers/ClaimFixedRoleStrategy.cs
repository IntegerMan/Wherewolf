using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimFixedRoleStrategy(GameRole role, Func<Player, GameState, SpecificRoleClaim> specificClaim) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState) 
        => role;

    public SpecificRoleClaim GetSpecificRoleClaim(Player player, GameState gameState, SpecificRoleClaim[] possibleClaims,
        GameRole initialClaim)
    {
        return specificClaim(player, gameState);
    }
}