using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Social;
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
        GameSetup gameSetup = new(new NonShuffler());
        gameSetup.SetPlayers(
            new Player("Matt", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Insomniac, (p,_) => new InsomniacWakeClaim(p, GameRole.Insomniac)),"Rufus")),
            new Player("Rufus", new RandomController()),
            new Player("Jimothy", new RandomController())
        );
        gameSetup.AddRoles(
            GameRole.Robber, 
            GameRole.Insomniac, 
            GameRole.Villager, GameRole.Villager, 
            GameRole.Werewolf, GameRole.Werewolf
        );
        GameState gameState = gameSetup.StartGame();
        
        // Act
        gameState.RunToEndOfNight(s => gameState = s);
        
        // Assert
        Player matt = gameState.Players.Single(p => p.Name == "Matt");
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(matt);
        
        SlotRoleProbabilities endProbabilities = playerProbs.GetCurrentProbabilities(gameState.GetSlot(matt));
        endProbabilities[GameRole.Insomniac].Probability.ShouldBe(1);
        endProbabilities[GameRole.Robber].Probability.ShouldBe(0);
        endProbabilities[GameRole.Villager].Probability.ShouldBe(0);
        endProbabilities[GameRole.Werewolf].Probability.ShouldBe(0);
    }
    
    [Fact]
    public void RobberStealingInsomniacShouldNotThinkTheyStartedAsVillager()
    {
        // Arrange
        GameSetup gameSetup = new(new NonShuffler());
        gameSetup.SetPlayers(
            new Player("Matt", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Insomniac, (p, _) => new InsomniacWakeClaim(p, GameRole.Insomniac) ), "Rufus")),
            new Player("Rufus", new RandomController()),
            new Player("Jimothy", new RandomController())
        );
        gameSetup.AddRoles(
            GameRole.Robber, 
            GameRole.Insomniac, 
            GameRole.Villager, GameRole.Villager, 
            GameRole.Werewolf, GameRole.Werewolf
        );
        GameState gameState = gameSetup.StartGame();
        
        // Act
        gameState.RunToEndOfNight(s => gameState = s);
        
        // Assert
        Player matt = gameState.Players.Single(p => p.Name == "Matt");
        PlayerProbabilities playerProbs = gameState.CalculateProbabilities(matt);
        SlotRoleProbabilities startProbs = playerProbs.GetStartProbabilities(gameState.GetSlot(matt));
        startProbs[GameRole.Robber].Probability.ShouldBe(1);
        startProbs[GameRole.Insomniac].Probability.ShouldBe(0);
        startProbs[GameRole.Villager].Probability.ShouldBe(0);
        startProbs[GameRole.Werewolf].Probability.ShouldBe(0);
    }    
}