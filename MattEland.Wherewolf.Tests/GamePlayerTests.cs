using MattEland.Wherewolf.Controllers;
using Shouldly;

namespace MattEland.Wherewolf.Tests;

public class GamePlayerTests
{
    [Fact]
    public void GameShouldStartWithoutPlayers()
    {
        // Arrange
        
        // Act
        Game game = new();

        // Assert
        game.Players.ShouldBeEmpty();
    }
    
    [Fact]
    public void AddingAPlayerShouldAddThePlayer()
    {
        // Arrange
        Game game = new();
        Player player = new Player("Test", new RandomController());
        
        // Act
        game.AddPlayer(player);

        // Assert
        game.Players.ShouldNotBeEmpty();
        game.Players.ShouldContain(player);
    }    
    
    [Fact]
    public void AddingAPlayerShouldAddThePlayerOnlyOnce()
    {
        // Arrange
        Game game = new();
        Player player = new Player("Test", new RandomController());
        game.AddPlayer(player);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() =>
        {
            game.AddPlayer(player);
        });

        // Assert
        game.Players.ShouldNotBeEmpty();
        game.Players.ShouldContain(player);
        game.Players.Count().ShouldBe(1);
    }        
}