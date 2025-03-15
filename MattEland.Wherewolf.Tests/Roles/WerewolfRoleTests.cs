using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Roles;

public class WerewolfRoleTests : RoleTestBase
{
    [Fact]
    public void LoneWerewolfShouldHaveLookedAtCardInCenterEvent()
    {
        // Arrange
        GameManager game = CreateTestGameManager(            
            GameRole.Werewolf, // This will go to our player
            GameRole.Villager,
            GameRole.Villager,
            // Center Cards
            GameRole.Villager,
            GameRole.Werewolf,
            GameRole.Villager
        );
        game.RunToEnd();
        Player player = game.Players.First();

        // Act
        IEnumerable<IGameEvent> observedEvents = game.EventsForPlayer(player);

        // Assert
        observedEvents.ShouldNotBeEmpty();
        // TODO: Something about running parallel tests seems to get this particular event to be skipped from callbacks
        observedEvents.OfType<LoneWolfLookedAtSlotEvent>().Count().ShouldBe(1);
    }
    

    [Fact]
    public void LoneWerewolfShouldHaveCertainKnowledgeOfStartingRoleOfTheCardTheyLookedAt()
    {
        // Arrange
        GameSetup setup = new(new NonShuffler());
        setup.AddRoles(            
            GameRole.Werewolf, // This will go to our player
            GameRole.Villager,
            GameRole.Villager,
            // Center Cards
            GameRole.Werewolf,
            GameRole.Villager,
            GameRole.Villager
        );
        setup.AddPlayers(
            new Player("A", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Villager, (p,_) => new VillagerNoActionClaim(p)), "Center 2", "B")),
            new Player("B", new RandomController()),
            new Player("C", new RandomController())
        );
        GameManager game = new GameManager(setup);
        game.RunToEnd();
        Player player = game.Players.Single(p => p.Name == "A");
        GameState gameState = game.CurrentState;
        
        // Act
        SlotRoleProbabilities slotProbabilities = gameState.CalculateProbabilities(player)
            .GetCurrentProbabilities(gameState["Center 2"]);

        // Assert
        IEnumerable<IGameEvent> observedEvents = game.EventsForPlayer(player);
        observedEvents.OfType<LoneWolfLookedAtSlotEvent>().Single().SlotName.ShouldBe("Center 2");
        slotProbabilities.Role[GameRole.Werewolf].Probability.ShouldBe(0);
        slotProbabilities.Role[GameRole.Villager].Probability.ShouldBe(1);
    }    
    
    [Fact]
    public void DualWerewolfShouldHaveSawOtherWerewolvesEvent()
    {
        // Arrange
        GameManager game = CreateTestGameManager(            
            GameRole.Werewolf, // This will go to player1
            GameRole.Werewolf, // This will go to player2
            GameRole.Villager,
            // Center Cards
            GameRole.Villager,
            GameRole.Villager,
            GameRole.Villager
        );
        game.RunToEnd();
        Player player1 = game.Players.First();
        Player player2 = game.Players.Skip(1).First();

        // Act
        IEnumerable<IGameEvent> p1ObservedEvents = game.EventsForPlayer(player1);
        IEnumerable<IGameEvent> p2ObservedEvents = game.EventsForPlayer(player2);

        // Assert
        p1ObservedEvents.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1);
        p2ObservedEvents.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1);
        game.Events.OfType<SawOtherWolvesEvent>().Count().ShouldBe(1); // 1 shared event
    }    
}