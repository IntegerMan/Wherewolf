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
        PlayerProbabilities playerProbs = gameState.Root.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.Root.GetPlayerSlot(player));

        // Assert
        playerProbabilities.Role["Robber"].ShouldBe(1);
    }

    [Fact]
    public void RobberShouldBeCertainTheyEndedAsStolenRoleWithNoOtherCardSwappingRoles()
    {
        // Arrange
        GameState gameState = RunRobberGame(); // Player moves from Robber -> Werewolf
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        playerProbabilities["Robber"].ShouldBe(0);
        playerProbabilities["Werewolf"].ShouldBe(1);
    }
    
    [Fact]
    public void RobberShouldHaveARobbedEvent()
    {
        // Arrange
        GameState gameState = RunRobberGame();
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        IEnumerable<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player));

        // Assert
        observedEvents.OfType<RobbedPlayerEvent>().Count().ShouldBe(1);
    }
    
    [Fact]
    public void RobberTargetShouldNotHaveARobbedEvent()
    {
        // Arrange
        GameState gameState = RunRobberGame();
        Player player = gameState.Players.Single(p => p.Name == "Target");

        // Act
        IEnumerable<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player));

        // Assert
        observedEvents.OfType<RobbedPlayerEvent>().Count().ShouldBe(0);
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