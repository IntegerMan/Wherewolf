using MattEland.Wherewolf.Roles;
using Shouldly;

namespace MattEland.Wherewolf.Tests;

public class GameRoleTests
{
    [Fact]
    public void GameShouldStartWithoutRoles()
    {
        // Arrange
        
        // Act
        Game game = new();

        // Assert
        game.Roles.ShouldBeEmpty();
    }
    
    [Fact]
    public void AddingARoleShouldAddIt()
    {
        // Arrange
        Game game = new();
        GameRole role = new VillagerRole();
        
        // Act
        game.AddRole(role);

        // Assert
        game.Roles.ShouldNotBeEmpty();
        game.Roles.ShouldContain(role);
    }    
    
    [Fact]
    public void AddingARoleShouldAddTheRoleOnlyOnce()
    {
        // Arrange
        Game game = new();
        GameRole role = new VillagerRole();
        game.AddRole(role);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() =>
        {
            game.AddRole(role);
        });

        // Assert
        game.Roles.ShouldNotBeEmpty();
        game.Roles.ShouldContain(role);
        game.Roles.Count().ShouldBe(1);
    }        
}