using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimFixedRoleStrategy : IRoleClaimStrategy
{
    private readonly GameRole _role;

    public ClaimFixedRoleStrategy(GameRole role)
    {
        _role = role;
    }
    
    public GameRole GetRoleClaim(Player player, GameState gameState) 
        => _role;
}