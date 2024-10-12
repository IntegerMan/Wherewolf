using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Tests.Phases;

public class InitialRoleClaimPhaseTests : GameTestsBase
{
    [Fact]
    public void ClaimingARoleShouldAddTheRoleClaimedEvent()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);

        // Act
        GameState endState = gameSetup.StartGame().RunToEnd();

        // Assert
        endState.Events.OfType<StartRoleClaimedEvent>().Count().ShouldBe(gameSetup.Players.Count());
    }    
    
    [Fact]
    public void GamesShouldIncludeRoleInitialClaimPhase()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);

        // Act
        GameState initialState = gameSetup.StartGame();

        // Assert
        initialState.Phases.OfType<InitialRoleClaimPhase>().ShouldNotBeEmpty();
    }
}