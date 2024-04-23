using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

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
        DealtCardEvent dealtCardEvent = gameState.Events.Where(e => e.IsObservedBy(player)).OfType<DealtCardEvent>().Single();

        // Assert
        dealtCardEvent.Role.GetType().ShouldBe(typeof(VillagerRole));
        dealtCardEvent.Player.ShouldBe(player);
    }

}