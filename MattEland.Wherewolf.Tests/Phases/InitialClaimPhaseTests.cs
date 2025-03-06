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

        // Act
        GameState? endState = null;
        gameSetup.StartGame().RunToEnd(state => endState = state);

        // Assert
        endState.ShouldNotBeNull();
        endState.Claims.OfType<StartRoleClaimedEvent>().Count().ShouldBe(gameSetup.Players.Count());
        endState.Claims.Count().ShouldBe(gameSetup.Players.Count());
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