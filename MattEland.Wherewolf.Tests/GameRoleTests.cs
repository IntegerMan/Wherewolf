using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests;

public class GameRoleTests
{
    [Fact]
    public void GameShouldStartWithoutRoles()
    {
        // Arrange
        
        // Act
        GameSetup gameSetup = new();

        // Assert
        gameSetup.Roles.ShouldBeEmpty();
    }
    
    [Fact]
    public void AddingARoleShouldAddIt()
    {
        // Arrange
        GameSetup gameSetup = new();
        GameRole role = GameRole.Villager;
        
        // Act
        gameSetup.AddRole(role);

        // Assert
        gameSetup.Roles.ShouldNotBeEmpty();
        gameSetup.Roles.ShouldContain(role);
    }
}