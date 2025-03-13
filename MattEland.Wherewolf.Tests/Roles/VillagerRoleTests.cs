using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Roles;

public class VillagerRoleTests : RoleTestBase
{
    [Fact]
    public void VillagerShouldGetDealtVillagerEvent()
    {
        // Arrange
        GameState gameState = TestingSetups.VillagersOnlyGame().StartGame(new NonShuffler());
        Player player = gameState.Players.First();

        // Act
        DealtCardEvent dealtCardEvent = gameState.Events.Where(e => e.IsObservedBy(player)).OfType<DealtCardEvent>().Single();

        // Assert
        dealtCardEvent.Role.ShouldBe(GameRole.Villager);
        dealtCardEvent.SlotName.ShouldBe(player.Name);
    }

}