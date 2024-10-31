using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public interface IRoleClaimStrategy
{
    GameRole GetRoleClaim(Player player, GameState gameState);
}