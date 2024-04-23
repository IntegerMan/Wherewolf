using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

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
        List<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player)).ToList();

        // Assert
        observedEvents.ShouldNotBeEmpty();
        observedEvents.OfType<LoneWolfEvent>().Count().ShouldBe(1);
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
        List<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player)).ToList();

        // Assert
        observedEvents.ShouldNotBeEmpty();
        observedEvents.OfType<LoneWolfLookedAtSlotEvent>().Count().ShouldBe(1);
    }
    

    [Fact]
    public void LoneWerewolfShouldHaveCertainKnowledgeOfStartingRoleOfTheCardTheyLookedAt()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddRoles(            
            new WerewolfRole(), // This will go to our player
            new VillagerRole(),
            new VillagerRole(),
            // Center Cards
            new WerewolfRole(),
            new VillagerRole(),
            new VillagerRole()
        );
        setup.AddPlayers(
            new Player("A", new FixedSelectionController("Center 2")),
            new Player("B", new RandomController()),
            new Player("C", new RandomController())
        );
        GameState gameState = setup.StartGame(new NonShuffler()).RunToEnd();
        Player player = gameState.Players.Single(p => p.Name == "A");
        
        // Act
        SlotRoleProbabilities slotProbabilities = gameState.CalculateProbabilities(player).GetSlotProbabilities(gameState.GetSlot("Center 2"));

        // Assert
        List<GameEvent> observedEvents = gameState.Events.Where(e => e.IsObservedBy(player)).ToList();
        observedEvents.OfType<LoneWolfLookedAtSlotEvent>().Single().Slot.Name.ShouldBe("Center 2");
        slotProbabilities.Role["Werewolf"].ShouldBe(0);
        slotProbabilities.Role["Villager"].ShouldBe(1);
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
        List<GameEvent> p1ObservedEvents = gameState.Events.Where(e => e.IsObservedBy(player1)).ToList();
        List<GameEvent> p2ObservedEvents = gameState.Events.Where(e => e.IsObservedBy(player2)).ToList();

        // Assert
        p1ObservedEvents.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1);
        p2ObservedEvents.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1);
        gameState.Events.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1); // 1 shared event
    }    
}