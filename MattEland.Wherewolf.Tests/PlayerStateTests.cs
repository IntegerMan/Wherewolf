using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests;

public class PlayerStateTests
{
    [Fact]
    public void PlayerStateShouldHavePlayer()
    {
        // Arrange
        Player player = new("A", new RandomController());
        
        // Act
        PlayerState playerState = new(player);
        
        // Assert
        playerState.Player.ShouldBe(player);
    }

    [Fact]
    public void PlayerStateObservingEventShouldListItInObservedEvents()
    {
        // Arrange
        Player player = new Player("A", new RandomController());
        PlayerState playerState = new(player);
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