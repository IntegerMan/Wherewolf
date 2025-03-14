using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

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
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.Root.GetSlot(player));

        // Assert
        playerProbabilities.Role[GameRole.Robber].Probability.ShouldBe(1);
    }

    [Fact]
    public void RobberShouldBeCertainTheyEndedAsStolenRoleWithNoOtherCardSwappingRoles()
    {
        // Arrange
        GameState gameState = RunRobberGame(); // Player moves from Robber -> Werewolf
        Player player = gameState.Players.Single(p => p.Name == "Player");

        // Act
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(player);
        SlotRoleProbabilities playerProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetSlot(player));

        // Assert
        playerProbabilities[GameRole.Robber].Probability.ShouldBe(0);
        playerProbabilities[GameRole.Werewolf].Probability.ShouldBe(1);
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
        GameSetup setup = new(new NonShuffler());
        setup.SetPlayers(
            new Player("Player", new FixedSelectionController(new ClaimStartingRoleStrategy(), "Target", "Other")),
            new Player("Target", new RandomController()),
            new Player("Other", new RandomController()));
        setup.AddRoles(
            GameRole.Robber, // this will go to our player
            GameRole.Werewolf,
            GameRole.Villager,
            // Center Cards
            GameRole.Villager,
            GameRole.Villager,
            GameRole.Werewolf
        );
        GameState? finalState = null;
        setup.StartGame().RunToEnd(s => finalState = s);

        return finalState!;
    }
}