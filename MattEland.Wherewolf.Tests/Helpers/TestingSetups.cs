using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Helpers;

public class TestingSetups
{
    public static GameSetup VillagersOnlyGame(int playerCount = 3)
    {
        GameSetup setup = new();
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
}