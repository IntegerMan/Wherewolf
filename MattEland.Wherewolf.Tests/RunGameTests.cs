namespace MattEland.Wherewolf.Tests;

public class RunGameTests : GameTestsBase
{
    [Fact]
    public void RunningGameStateToEndShouldProduceGameEvents()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        AddMinimumRequiredRoles(game);
        GameState startState = game.StartGame();
        int originalEventCount = startState.Events.Count();

        // Act
        GameState finalState = startState.RunToEnd();
        
        // Assert
        finalState.ShouldNotBeNull();
        finalState.IsGameOver.ShouldBeTrue();
        finalState.ShouldNotBe(startState);
        finalState.Events.Count().ShouldBeGreaterThan(originalEventCount);
    }
}