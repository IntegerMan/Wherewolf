using MattEland.Wherewolf.Roles;

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
        GameRole role = new VillagerRole();
        
        // Act
        gameSetup.AddRole(role);

        // Assert
        gameSetup.Roles.ShouldNotBeEmpty();
        gameSetup.Roles.ShouldContain(role);
    }    
    
    [Fact]
    public void AddingARoleShouldAddTheRoleOnlyOnce()
    {
        // Arrange
        GameSetup gameSetup = new();
        GameRole role = new VillagerRole();
        gameSetup.AddRole(role);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() =>
        {
            gameSetup.AddRole(role);
        });

        // Assert
        gameSetup.Roles.ShouldNotBeEmpty();
        gameSetup.Roles.ShouldContain(role);
        gameSetup.Roles.Count().ShouldBe(1);
    }        
}