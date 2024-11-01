using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

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
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetSlot(player));

        // Assert
        playerProbabilities.Role[GameRole.Insomniac].Probability.ShouldBe(1);
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
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetSlot(player));

        // Assert
        playerProbabilities[GameRole.Insomniac].Probability.ShouldBe(1);
    }
    
    [Fact]
    public void InsomniacShouldBeCertainOfFinalRoleWhenRobbed()
    {
        // Arrange
        GameState gameState = RunInsomniacGame(robPlayer: true);
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetSlot(player));

        // Assert
        gameState.GetSlot(player).Role.ShouldBe(GameRole.Robber);
        playerProbabilities[GameRole.Robber].Probability.ShouldBe(1);
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
        SlotRoleProbabilities thiefProbabilities = playerProbs.GetStartProbabilities(gameState.GetSlot(robber));

        // Assert
        thiefProbabilities[GameRole.Robber].Probability.ShouldBeLessThan(1);
    }
    
    private static GameState RunInsomniacGame(bool robPlayer)
    {
        GameSetup setup = new();
        setup.AddPlayers(
            new Player("Player", new RandomController()),
            new Player("Thief", new FixedSelectionController(new ClaimStartingRoleStrategy(), robPlayer ? "Player" : "Other", "Player")),
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