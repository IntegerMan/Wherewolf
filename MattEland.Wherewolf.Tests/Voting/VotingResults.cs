using MattEland.Wherewolf.Controllers;

namespace MattEland.Wherewolf.Tests.Voting;

public class VoteTabulationTests
{
    [Fact]
    public void VotingResultsShouldTabulateVotesForSinglePlayer()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        Dictionary<Player, Player?> votes = new Dictionary<Player, Player?>
        {
            [a] = b,
            [b] = c,
            [c] = b
        };

        // Act
        var results = GameSetup.GetVotingResults(votes);
        
        // Assert
        results.ShouldNotBeNull();
        results.Keys.Count.ShouldBe(3);
        results[a].ShouldBe(0);
        results[b].ShouldBe(2);
        results[c].ShouldBe(1);
    }
    
    [Fact]
    public void VotingResultsShouldTabulateAllSkip()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        Dictionary<Player, Player?> votes = new Dictionary<Player, Player?>
        {
            [a] = null,
            [b] = null,
            [c] = null
        };

        // Act
        var results = GameSetup.GetVotingResults(votes);
        
        // Assert
        results.ShouldNotBeNull();
        results.Keys.Count.ShouldBe(3);
        results[a].ShouldBe(0);
        results[b].ShouldBe(0);
        results[c].ShouldBe(0);
    }    
    
    [Fact]
    public void VotingResultsShouldTabulateCircleVote()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        Dictionary<Player, Player?> votes = new Dictionary<Player, Player?>
        {
            [a] = b,
            [b] = c,
            [c] = a
        };

        // Act
        var results = GameSetup.GetVotingResults(votes);
        
        // Assert
        results.ShouldNotBeNull();
        results.Keys.Count.ShouldBe(3);
        results[a].ShouldBe(1);
        results[b].ShouldBe(1);
        results[c].ShouldBe(1);
    }
    
    [Fact]
    public void VotingResultsShouldTabulateTieVotes()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        Player d = new("D", new RandomController());
        Dictionary<Player, Player?> votes = new Dictionary<Player, Player?>
        {
            [a] = b,
            [b] = c,
            [c] = b,
            [d] = c
        };

        // Act
        var results = GameSetup.GetVotingResults(votes);
        
        // Assert
        results.ShouldNotBeNull();
        results.Keys.Count.ShouldBe(4);
        results[a].ShouldBe(0);
        results[b].ShouldBe(2);
        results[c].ShouldBe(2);
        results[d].ShouldBe(0);
    }
}