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
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        playerProbabilities.Role[GameRole.Insomniac].ShouldBe(1);
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
        observedEvents.OfType<InsomniacSawFinalCardEvent>().Single().Role.ShouldBe(GameRole.Insomniac);
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
        observedEvents.OfType<InsomniacSawFinalCardEvent>().Single().Role.ShouldBe(GameRole.Robber);
    }
    
    [Fact]
    public void InsomniacShouldBeCertainOfFinalRoleWhenStillInsomniac()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: false);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        playerProbabilities[GameRole.Insomniac].ShouldBe(1);
    }
    
    [Fact]
    public void InsomniacShouldBeCertainOfFinalRoleWhenRobbed()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: true);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetPlayerSlot(player));

        // Assert
        gameState.GetPlayerSlot(player).Role.ShouldBe(GameRole.Robber);
        playerProbabilities[GameRole.Robber].ShouldBe(1);
    }
        
    [Fact]
    public void InsomniacShouldNotBeCertainOfStartingRobberWhenRobbed()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: true);
        Player insomniac = gameState.Players.Single(p => p.Name == "Player");
        Player robber = gameState.Players.Single(p => p.Name == "Thief");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(insomniac);
        SlotRoleProbabilities thiefProbabilities = playerProbs.GetStartProbabilities(gameState.GetPlayerSlot(robber));

        // Assert
        thiefProbabilities[GameRole.Robber].ShouldBeLessThan(1);
    }
    
    private static GameState RunInsomniacGame(bool robPlayer)
    {
        GameSetup setup = new();
        setup.AddPlayers(
            new Player("Player", new RandomController()),
            new Player("Thief", new FixedSelectionController(robPlayer ? "Player" : "Other", "Player")),
            new Player("Other", new RandomController()));
        setup.AddRoles(
            GameRole.Insomniac, // this will go to our player
            GameRole.Robber,
            GameRole.Werewolf,
            // Center Cards
            GameRole.Villager,
            GameRole.Villager,
            GameRole.Werewolf
        );
        return setup.StartGame(new NonShuffler()).RunToEnd();
    }
}