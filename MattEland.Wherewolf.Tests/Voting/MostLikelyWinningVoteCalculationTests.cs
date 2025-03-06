using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Voting;

public class MostLikelyWinningVoteCalculationTests : GameTestsBase
{
    [Fact]
    public void WerewolfWithOtherWerewolfShouldKnowToVoteForVillager()
    {
        // Arrange
        GameSetup setup = new();
        AddMinimumRequiredPlayers(setup);
        AddMinimumRequiredRoles(setup); // WW, WW, Villager
        GameState gameState = setup.StartGame(new NonShuffler());
        gameState.RunToEndOfNight(s => gameState = s);
        Random rand = new();
        
        // Act
        Player vote = VotingHelper.GetMostLikelyWinningVote(gameState, gameState.Players.First(), rand);
        
        // Assert
        vote.Name.ShouldBe("C");
    }
}