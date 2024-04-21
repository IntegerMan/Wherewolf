using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Roles;

public class RobberRoleTests : RoleTestBase
{
    [Fact]
    public void RobberShouldBeCertainTheyStartedAsRobber()
    {
        // Arrange
        GameState gameState = RunRobberGame();
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerState playerState = gameState.GetPlayerStates(player);
        SlotRoleProbabilities playerProbabilities = playerState.Probabilities.GetSlotProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        playerProbabilities.StartRole["Robber"].ShouldBe(1);
    }

    [Fact]
    public void RobberShouldHaveARobbedEvent()
    {
        // Arrange
        GameState gameState = RunRobberGame();
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerState playerState = gameState.GetPlayerStates(player);

        // Assert
        playerState.ObservedEvents.OfType<RobbedPlayerEvent>().Count().ShouldBe(1);
    }
    
    [Fact]
    public void RobberTargetShouldNotHaveARobbedEvent()
    {
        // Arrange
        GameState gameState = RunRobberGame();
        Player player = gameState.Players.Single(p => p.Name == "Target");

        // Act
        PlayerState playerState = gameState.GetPlayerStates(player);

        // Assert
        playerState.ObservedEvents.OfType<RobbedPlayerEvent>().Count().ShouldBe(0);
    }

    private static GameState RunRobberGame()
    {
        GameSetup setup = new();
        setup.AddPlayers(
            new Player("Player", new FixedSelectionController("Target")),
            new Player("Target", new RandomController()),
            new Player("Other", new RandomController()));
        setup.AddRoles(
            new RobberRole(), // this will go to our player
            new WerewolfRole(),
            new VillagerRole(),
            // Center Cards
            new VillagerRole(),
            new VillagerRole(),
            new WerewolfRole()
        );
        return setup.StartGame(new NonShuffler()).RunToEnd();
    }
}