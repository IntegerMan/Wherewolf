using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests;

public class GamePlayerTests
{
    [Fact]
    public void GameShouldStartWithoutPlayers()
    {
        // Arrange
        
        // Act
        GameSetup gameSetup = new();

        // Assert
        gameSetup.Players.ShouldBeEmpty();
    }
    
    [Fact]
    public void AddingAPlayerShouldAddThePlayer()
    {
        // Arrange
        GameSetup gameSetup = new();
        Player player = new("Test", new RandomController(new ClaimStartingRoleStrategy()));
        
        // Act
        gameSetup.SetPlayers(player);

        // Assert
        gameSetup.Players.ShouldNotBeEmpty();
        gameSetup.Players.ShouldContain(player);
    }    
}