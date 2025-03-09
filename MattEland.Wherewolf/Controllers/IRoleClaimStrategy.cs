using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public interface IRoleClaimStrategy
{
    GameRole GetRoleClaim(Player player, GameState gameState);
    SpecificRoleClaim GetSpecificRoleClaim(Player player, GameState gameState, SpecificRoleClaim[] possibleClaims, GameRole initialClaim);
}