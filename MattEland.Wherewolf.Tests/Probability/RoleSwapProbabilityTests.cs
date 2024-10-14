using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Probability;

public class RoleSwapProbabilityTests
{
    [Fact]
    public void RobberStealingInsomniacShouldNotThinkTheyAreCurrentlyWerewolf()
    {
        // Arrange
        GameSetup gameSetup = new();
        gameSetup.AddPlayers(
            new Player("Matt", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Insomniac),"Rufus")),
            new Player("Rufus", new RandomController()),
            new Player("Jimothy", new RandomController())
        );
        gameSetup.AddRoles(
            GameRole.Robber, 
            GameRole.Insomniac, 
            GameRole.Villager, GameRole.Villager, 
            GameRole.Werewolf, GameRole.Werewolf
        );
        GameState gameState = gameSetup.StartGame(new NonShuffler());
        
        // Act
        gameState = gameState.RunToEndOfNight();
        
        // Assert
        Player matt = gameState.Players.Single(p => p.Name == "Matt");
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(matt);
        
        SlotRoleProbabilities endProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetPlayerSlot(matt));
        endProbabilities[GameRole.Insomniac].ShouldBe(1);
        endProbabilities[GameRole.Robber].ShouldBe(0);
        endProbabilities[GameRole.Villager].ShouldBe(0);
        endProbabilities[GameRole.Werewolf].ShouldBe(0);
    }
    
    [Fact]
    public void RobberStealingInsomniacShouldNotThinkTheyStartedAsVillager()
    {
        // Arrange
        GameSetup gameSetup = new();
        gameSetup.AddPlayers(
            new Player("Matt", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Insomniac), "Rufus")),
            new Player("Rufus", new RandomController()),
            new Player("Jimothy", new RandomController())
        );
        gameSetup.AddRoles(
            GameRole.Robber, 
            GameRole.Insomniac, 
            GameRole.Villager, GameRole.Villager, 
            GameRole.Werewolf, GameRole.Werewolf
        );
        GameState gameState = gameSetup.StartGame(new NonShuffler());
        
        // Act
        gameState = gameState.RunToEndOfNight();
        
        // Assert
        Player matt = gameState.Players.Single(p => p.Name == "Matt");
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(matt);
        SlotRoleProbabilities startProbs = playerProbs.GetStartProbabilities(gameState.GetPlayerSlot(matt));
        startProbs[GameRole.Robber].ShouldBe(1);
        startProbs[GameRole.Insomniac].ShouldBe(0);
        startProbs[GameRole.Villager].ShouldBe(0);
        startProbs[GameRole.Werewolf].ShouldBe(0);
    }    
}