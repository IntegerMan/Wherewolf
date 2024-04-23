using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Roles;

public class InsomniacRoleTests : RoleTestBase
{
    [Fact]
    public void InsomniacShouldBeCertainTheyStartedAsInsomniac()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: false);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetSlotProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        playerProbabilities.Role["Insomniac"].ShouldBe(1);
    }
    
    [Fact]
    public void InsomniacShouldReceiveSawOwnCardEventWhenNotSwapped()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: false);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        List<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player)).ToList();

        // Assert
        observedEvents.OfType<InsomniacSawFinalCardEvent>().Count().ShouldBe(1);
        observedEvents.OfType<InsomniacSawFinalCardEvent>().Single().Role.Name.ShouldBe("Insomniac");
    }
    
    [Fact]
    public void InsomniacShouldReceiveSawOwnCardEventWhenRobbed()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: true);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        List<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player)).ToList();

        // Assert
        observedEvents.OfType<InsomniacSawFinalCardEvent>().Count().ShouldBe(1);
        observedEvents.OfType<InsomniacSawFinalCardEvent>().Single().Role.Name.ShouldBe("Robber");
    }
    
    [Fact]
    public void InsomniacShouldBeCertainOfFinalRoleWhenStillInsomniac()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: false);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetSlotProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        playerProbabilities["Insomniac"].ShouldBe(1);
    }
    
    [Fact]
    public void InsomniacShouldBeCertainOfFinalRoleWhenRobbed()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: true);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetSlotProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        gameState.GetPlayerSlot(player).CurrentRole.Name.ShouldBe("Robber");
        playerProbabilities["Robber"].ShouldBe(1);
    }
    
    private static GameState RunInsomniacGame(bool robPlayer)
    {
        GameSetup setup = new();
        setup.AddPlayers(
            new Player("Player", new RandomController()),
            new Player("Thief", new FixedSelectionController(robPlayer ? "Player" : "Other")),
            new Player("Other", new RandomController()));
        setup.AddRoles(
            new InsomniacRole(), // this will go to our player
            new RobberRole(),
            new WerewolfRole(),
            // Center Cards
            new VillagerRole(),
            new VillagerRole(),
            new WerewolfRole()
        );
        return setup.StartGame(new NonShuffler()).RunToEnd();
    }
}