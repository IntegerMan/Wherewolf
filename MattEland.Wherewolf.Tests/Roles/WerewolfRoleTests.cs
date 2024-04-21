using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests.Roles;

public class WerewolfRoleTests : RoleTestBase
{
    [Fact]
    public void LoneWerewolfShouldHaveLoneWerewolfEvent()
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
        playerState.ObservedEvents.OfType<LoneWolfEvent>().Count().ShouldBe(1);
    }

    [Fact]
    public void LoneWerewolfShouldHaveLookedAtCardInCenterEvent()
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
        playerState.ObservedEvents.OfType<LoneWolfLookedAtSlotEvent>().Count().ShouldBe(1);
    }
    
    [Fact]
    public void DualWerewolfShouldHaveSawOtherWerewolvesEvent()
    {
        // Arrange
        GameState gameState = CreateTestGameState(            
            new WerewolfRole(), // This will go to player1
            new WerewolfRole(), // This will go to player2
            new VillagerRole(),
            // Center Cards
            new VillagerRole(),
            new VillagerRole(),
            new VillagerRole()
        );
        gameState = gameState.RunToEnd();
        Player player1 = gameState.Players.First();
        Player player2 = gameState.Players.Skip(1).First();

        // Act
        PlayerState p1State = gameState.GetPlayerStates(player1);
        PlayerState p2State = gameState.GetPlayerStates(player2);

        // Assert
        p1State.ObservedEvents.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1);
        p2State.ObservedEvents.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1);
        gameState.Events.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1); // 1 shared event
    }    
}