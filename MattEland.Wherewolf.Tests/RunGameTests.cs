using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests;

public class RunGameTests : GameTestsBase
{
    [Fact]
    public void RunningGameStateToEndShouldProduceGameEvents()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        GameState startState = gameSetup.StartGame(new NonShuffler());
        int originalEventCount = startState.Events.Count();

        // Act
        GameState? finalState = null;
        startState.RunToEnd(s => finalState = s);
        
        // Assert
        finalState.ShouldNotBeNull();
        finalState.IsGameOver.ShouldBeTrue();
        finalState.ShouldNotBe(startState);
        finalState.Events.Count().ShouldBeGreaterThan(originalEventCount);
    }
}