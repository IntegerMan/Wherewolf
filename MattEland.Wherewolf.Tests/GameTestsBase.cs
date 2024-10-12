using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests;

public abstract class GameTestsBase
{
    protected static void AddMinimumRequiredPlayers(GameSetup gameSetup)
    {
        gameSetup.AddPlayers(
            new Player("A", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("B", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
    }
    
    protected static void AddMinimumRequiredRoles(GameSetup gameSetup)
    {
        gameSetup.AddRoles(
            GameRole.Werewolf, GameRole.Werewolf,
            GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager 
        );
    }
}