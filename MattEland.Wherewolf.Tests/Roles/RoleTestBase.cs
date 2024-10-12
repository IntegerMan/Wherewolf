using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Roles;

public abstract class RoleTestBase
{
    protected static GameSetup CreateTestGame(params GameRole[] roles)
    {
        GameSetup gameSetup = new();
        AddPlayersToGame(gameSetup);
        gameSetup.AddRoles(roles);
        
        return gameSetup;
    }
    
    protected static GameState CreateTestGameState(params GameRole[] roles)
    {
        GameSetup gameSetup = CreateTestGame(roles);

        return gameSetup.StartGame(new NonShuffler());
    }    

    private static void AddPlayersToGame(GameSetup gameSetup)
    {
        gameSetup.AddPlayers(
            new Player("A", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("B", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
    }
}