using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using Shouldly;

namespace MattEland.Wherewolf.Tests;

public class StartGameTests
{
    [Fact]
    public void StartGameShouldErrorIfNoPlayersOrRoles()
    {
        // Arrange
        Game game = new();
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => game.StartGame());
        
        // Assert (already asserted)
    }
    
    [Fact]
    public void StartGameShouldErrorIfNoPlayers()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredRoles(game);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => game.StartGame());
        
        // Assert (already asserted)
    }

    [Fact]
    public void StartGameShouldErrorIfNoRoles()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => game.StartGame());
        
        // Assert (already asserted)
    }
    
    [Fact]
    public void StartGameShouldErrorIfNotEnoughRoles()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        game.AddRoles(new VillagerRole(), new WerewolfRole(), new VillagerRole());
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => game.StartGame());
        
        // Assert (already asserted)
    }    
    
    [Fact]
    public void StartGameShouldErrorIfNoEvilRoles()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        game.AddRoles(new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole());
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => game.StartGame());
        
        // Assert (already asserted)
    }    
    
    [Fact]
    public void StartGameShouldErrorIfNoGoodRoles()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        game.AddRoles(new WerewolfRole(), new WerewolfRole(), new WerewolfRole(), new WerewolfRole(), new WerewolfRole(), new WerewolfRole());
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => game.StartGame());
        
        // Assert (already asserted)
    }        
    
    [Fact]
    public void StartGameShouldProduceGameStateWhenValid()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        AddMinimumRequiredRoles(game);
        
        // Act
        GameState state = game.StartGame();

        // Assert
        state.ShouldNotBeNull();
        state.CenterSlots.Count().ShouldBe(3);
        state.PlayerSlots.Count().ShouldBe(3);
        state.Players.Count().ShouldBe(3);
        state.Roles.Count().ShouldBe(6);
    }   
    
    [Fact]
    public void StartGameShouldResultInOneSlotPerPlayer()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        AddMinimumRequiredRoles(game);
        
        // Act
        GameState state = game.StartGame();

        // Assert
        state.ShouldNotBeNull();
        foreach (var player in game.Players)
        {
            state.PlayerSlots.Count(s => s.Player == player).ShouldBe(1);
            state.PlayerSlots.Count(s => s.Name == player.Name).ShouldBe(1);
            state.CenterSlots.Count(s => s.Player == player).ShouldBe(0);
        }
    }   
    
    [Fact]
    public void StartGameShouldResultInCorrectCenterSlotNames()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        AddMinimumRequiredRoles(game);
        
        // Act
        GameState state = game.StartGame();

        // Assert
        state.ShouldNotBeNull();
        state.CenterSlots[0].Name.ShouldBe("Center 1");
        state.CenterSlots[1].Name.ShouldBe("Center 2");
        state.CenterSlots[2].Name.ShouldBe("Center 3");
    }       

    [Fact]
    public void StartGameShouldResultInRoleBeingAssignedToOnlyOneSlot()
    {
        // Arrange
        Game game = new();
        AddMinimumRequiredPlayers(game);
        AddMinimumRequiredRoles(game);
        
        // Act
        GameState state = game.StartGame();

        // Assert
        state.ShouldNotBeNull();
        foreach (var role in game.Roles)
        {
            int playerRoles = state.PlayerSlots.Count(s => s.StartRole == role);
            int centerRoles = state.CenterSlots.Count(s => s.StartRole == role);
            
            Math.Max(playerRoles, centerRoles).ShouldBe(1);
            (playerRoles + centerRoles).ShouldBe(1);
        }
    }       
    private static void AddMinimumRequiredPlayers(Game game)
    {
        game.AddPlayers(
            new Player("A", new RandomController()), 
            new Player("B", new RandomController()), 
            new Player("C", new RandomController()));
    }
    
    private static void AddMinimumRequiredRoles(Game game)
    {
        game.AddRoles(
                new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), 
                new WerewolfRole(), new WerewolfRole()
            );
    }    
}