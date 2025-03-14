using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests;

public class StartGameTests : GameTestsBase
{
    [Fact]
    public void StartGameShouldErrorIfNoPlayersOrRoles()
    {
        // Arrange
        GameSetup gameSetup = new();
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => gameSetup.StartGame());
        
        // Assert (already asserted)
    }
    
    [Fact]
    public void StartGameShouldErrorIfNoPlayers()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredRoles(gameSetup);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => gameSetup.StartGame());
        
        // Assert (already asserted)
    }

    [Fact]
    public void StartGameShouldErrorIfNoRoles()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => gameSetup.StartGame());
        
        // Assert (already asserted)
    }
    
    [Fact]
    public void StartGameShouldErrorIfNotEnoughRoles()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        gameSetup.AddRoles(GameRole.Villager, GameRole.Werewolf, GameRole.Villager);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => gameSetup.StartGame());
        
        // Assert (already asserted)
    }    
    
    [Fact]
    public void StartGameShouldErrorIfNoEvilRoles()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        gameSetup.AddRoles(GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => gameSetup.StartGame());
        
        // Assert (already asserted)
    }    
    
    [Fact]
    public void StartGameShouldErrorIfNoGoodRoles()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        gameSetup.AddRoles(GameRole.Werewolf, GameRole.Werewolf, GameRole.Werewolf, GameRole.Werewolf, GameRole.Werewolf, GameRole.Werewolf);
        
        // Act
        ShouldThrowExtensions.ShouldThrow<InvalidOperationException>(() => gameSetup.StartGame());
        
        // Assert (already asserted)
    }        
    
    [Fact]
    public void StartGameShouldProduceGameStateWhenValid()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        
        // Act
        GameState state = gameSetup.StartGame();

        // Assert
        state.ShouldNotBeNull();
        state.CenterSlots.Length.ShouldBe(3);
        state.PlayerSlots.Length.ShouldBe(3);
        state.Players.Count().ShouldBe(3);
        state.Roles.Count().ShouldBe(6);
        state.IsGameOver.ShouldBeFalse();
    }
    
    [Fact]
    public void StartGameShouldResultInCorrectIndexesPerPlayer()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        
        // Act
        GameState state = gameSetup.StartGame();

        // Assert
        state.ShouldNotBeNull();
        List<Player> players = state.Players.ToList();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Order.ShouldBe(i);
        }
    }
    
    [Fact]
    public void StartGameShouldResultInOneSlotPerPlayer()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        
        // Act
        GameState state = gameSetup.StartGame();

        // Assert
        state.ShouldNotBeNull();
        foreach (var player in gameSetup.Players)
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
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        
        // Act
        GameState state = gameSetup.StartGame();

        // Assert
        state.ShouldNotBeNull();
        state.CenterSlots[0].Name.ShouldBe("Center 1");
        state.CenterSlots[1].Name.ShouldBe("Center 2");
        state.CenterSlots[2].Name.ShouldBe("Center 3");
    }  

    [Fact]
    public void StartGameShouldGenerateCardDealtEventsToAllSlots()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        
        // Act
        GameState state = gameSetup.StartGame();

        // Assert
        state.ShouldNotBeNull();
        foreach (var slot in state.AllSlots)
        {
            state.Events.OfType<DealtCardEvent>().ShouldContain(e => e.SlotName == slot.Name);
        }
    }
}