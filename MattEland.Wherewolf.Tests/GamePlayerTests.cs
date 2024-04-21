using MattEland.Wherewolf.Controllers;

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
        Player player = new Player("Test", new RandomController());
        
        // Act
        gameSetup.AddPlayer(player);

        // Assert
        gameSetup.Players.ShouldNotBeEmpty();
        gameSetup.Players.ShouldContain(player);
    }    
    
    [Fact]
    public void AddingAPlayerShouldAddThePlayerOnlyOnce()
    {
        // Arrange
        GameSetup gameSetup = new();
        Player player = new Player("Test", new RandomController());
        gameSetup.AddPlayer(player);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() =>
        {
            gameSetup.AddPlayer(player);
        });

        // Assert
        gameSetup.Players.ShouldNotBeEmpty();
        gameSetup.Players.ShouldContain(player);
        gameSetup.Players.Count().ShouldBe(1);
    }        
}