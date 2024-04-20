using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Roles;

public abstract class RoleTestBase
{
    protected static Game CreateTestGame(params GameRole[] roles)
    {
        Game game = new();
        AddPlayersToGame(game);
        game.AddRoles(roles);
        
        return game;
    }
    
    protected static GameState CreateTestGameState(params GameRole[] roles)
    {
        Game game = CreateTestGame(roles);

        return game.StartGame(new NonShuffler());
    }    

    private static void AddPlayersToGame(Game game)
    {
        game.AddPlayers(
            new Player("A", new RandomController()), 
            new Player("B", new RandomController()), 
            new Player("C", new RandomController()));
    }
}