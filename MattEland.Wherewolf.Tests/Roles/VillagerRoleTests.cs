using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;
using Shouldly;

namespace MattEland.Wherewolf.Tests.Roles;

public class VillagerRoleTests : RoleTestBase
{
    [Fact]
    public void VillagerShouldGetDealtVillagerEvent()
    {
        // Arrange
        GameState gameState = CreateTestGameState(            
            new VillagerRole(), // This will go to our player
            new VillagerRole(),
            new VillagerRole(),
            // Center Cards
            new VillagerRole(),
            new WerewolfRole(),
            new WerewolfRole()
        );
        Player player = gameState.Players.First();

        // Act
        PlayerState playerState = gameState.GetPlayerStates(player);

        // Assert
        playerState.ShouldNotBeNull();
        playerState.Player.ShouldBe(player);
        playerState.ObservedEvents.ShouldNotBeNull();
        playerState.ObservedEvents.ShouldNotBeEmpty();
        DealtCardEvent dealtCardEvent = playerState.ObservedEvents.OfType<DealtCardEvent>().Single();
        dealtCardEvent.Card.GetType().ShouldBe(typeof(VillagerRole));
        dealtCardEvent.Player.ShouldBe(player);
    }

}