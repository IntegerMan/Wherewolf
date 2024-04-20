using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;


namespace MattEland.Wherewolf.Tests.Roles;

public class WerewolfRoleTests : RoleTestBase
{
    [Fact]
    public void SoloWerewolfShouldHaveSoloWerewolfEvent()
    {
        // Arrange
        GameState gameState = CreateTestGameState(            
            new WerewolfRole(), // This will go to our player
            new VillagerRole(),
            new VillagerRole(),
            // Center Cards
            new VillagerRole(),
            new WerewolfRole(),
            new VillagerRole()
        );
        gameState = gameState.RunToEnd();
        Player player = gameState.Players.First();

        // Act
        PlayerState playerState = gameState.GetPlayerStates(player);

        // Assert
        playerState.ShouldNotBeNull();
        playerState.ObservedEvents.ShouldNotBeNull();
        playerState.ObservedEvents.OfType<SoloWolfEvent>().Count().ShouldBe(1);
    }
}