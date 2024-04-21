using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests.Helpers;

public class TestingSetups
{
    public static GameSetup VillagersOnlyGame(int playerCount = 3)
    {
        GameSetup setup = new();
        for (int i = 1; i <= playerCount; i++)
        {
            setup.AddPlayer(new Player($"Player {i}", new RandomController()));
            setup.AddRole(new VillagerRole());
        }
        
        setup.AddRole(new VillagerRole());
        setup.AddRole(new WerewolfRole());
        setup.AddRole(new WerewolfRole());

        return setup;
    }
}