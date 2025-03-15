using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Helpers;

public static class TestingSetups
{
    public static GameSetup VillagersOnlyGame(int playerCount = 3)
    {
        GameSetup setup = new(new NonShuffler());
        for (int i = 1; i <= playerCount; i++)
        {
            setup.AddPlayer(new Player($"Player {i}", new RandomController(new ClaimStartingRoleStrategy())));
            setup.AddRole(GameRole.Villager);
        }
        
        setup.AddRole(GameRole.Villager);
        setup.AddRole(GameRole.Werewolf);
        setup.AddRole(GameRole.Werewolf);

        return setup;
    }
    
    public static GameState RunGame(this GameSetup setup)
    {
        GameManager game = new(setup);
        game.RunToEnd();
        return game.CurrentState;
    }
}