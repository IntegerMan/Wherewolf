using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Setup;

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
        GameManager game = new(gameSetup);
        // Act
        game.RunToEnd();

        // Assert
        GameState endState = game.CurrentState;
        endState.ShouldNotBeNull();
        endState.Claims.OfType<StartRoleClaimedEvent>().Count().ShouldBe(gameSetup.Players.Count());
    }    
    
    [Fact]
    public void GamesShouldIncludeRoleInitialClaimPhase()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);

        // Act
        GamePhase[] phases = gameSetup.Phases;

        // Assert
        phases.OfType<InitialRoleClaimPhase>().ShouldNotBeEmpty();
    }
}