using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Helpers;

public static class TestingSetups
{
    public static GameSetup VillagersOnlyGame(int playerCount = 3)
    {
        GameSetup setup = new(new NonShuffler());
        
        RandomController controller = new RandomController(new ClaimStartingRoleStrategy());
        setup.SetPlayers(Enumerable.Range(1, playerCount)
            .Select(i => new Player($"Player {i}", controller))
            .ToArray());
        
        setup.AddRoles(Enumerable.Repeat(GameRole.Villager, playerCount).ToArray());
        setup.AddRoles(GameRole.Villager, GameRole.Werewolf, GameRole.Werewolf);

        return setup;
    }
}