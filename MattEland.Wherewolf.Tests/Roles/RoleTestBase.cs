using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Roles;

public abstract class RoleTestBase
{
    protected GameManager CreateTestGameManager(params GameRole[] roles)
    {
        GameSetup gameSetup = new(new NonShuffler());
        RandomController controller = new(new ClaimStartingRoleStrategy());
        
        gameSetup.AddPlayers(
            new Player("A", controller), 
            new Player("B", controller), 
            new Player("C", controller));
        
        gameSetup.AddRoles(roles);

        return new GameManager(gameSetup);
    }
}