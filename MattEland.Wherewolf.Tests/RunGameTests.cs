using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests;

public class RunGameTests : GameTestsBase
{
    [Fact]
    public void RunningGameStateToEndShouldProduceGameEvents()
    {
        // Arrange
        GameSetup gameSetup = new(new NonShuffler());
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        GameManager game = new(gameSetup);
        GameState startState = game.CurrentState;
        int originalEventCount = game.Events.Count();

        // Act
        game.RunToEnd();
        
        // Assert
        GameState finalState = game.CurrentState;
        finalState.ShouldNotBeNull();
        finalState.IsGameOver.ShouldBeTrue();
        finalState.ShouldNotBe(startState);
        finalState.Events.Count().ShouldBeGreaterThan(originalEventCount);
    }
}