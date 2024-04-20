using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests;

public abstract class GameTestsBase
{
    protected static void AddMinimumRequiredPlayers(Game game)
    {
        game.AddPlayers(
            new Player("A", new RandomController()), 
            new Player("B", new RandomController()), 
            new Player("C", new RandomController()));
    }
    
    protected static void AddMinimumRequiredRoles(Game game)
    {
        game.AddRoles(
            new WerewolfRole(), new WerewolfRole(),
            new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole() 
        );
    }
}