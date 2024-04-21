using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests;

public class PlayerStateTests
{
    [Fact]
    public void PlayerStateShouldHavePlayer()
    {
        // Arrange
        GameState startState = TestingSetups.VillagersOnlyGame().StartGame();
        Player player = startState.Players.First();
        
        // Act
        PlayerState playerState = new(player, startState);
        
        // Assert
        playerState.Player.ShouldBe(player);
    }

    [Fact]
    public void PlayerStateObservingEventShouldListItInObservedEvents()
    {
        // Arrange
        GameState startState = TestingSetups.VillagersOnlyGame().StartGame();
        Player player = startState.Players.First();
        PlayerState playerState = new(player, startState);
        VillagerRole villagerRole = new VillagerRole();
        GameSlot slot = new GameSlot(player.Name, villagerRole);
        DealtCardEvent dealtEvent = new DealtCardEvent(villagerRole, slot);

        // Act
        playerState.AddEvent(dealtEvent);
        
        // Assert
        playerState.ObservedEvents.ShouldNotBeNull();
        playerState.ObservedEvents.ShouldNotBeEmpty();
        playerState.ObservedEvents.Count().ShouldBe(1);
        playerState.ObservedEvents.ShouldContain(dealtEvent);
    }
}