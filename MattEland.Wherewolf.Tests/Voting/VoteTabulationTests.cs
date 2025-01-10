using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Tests.Voting;

public class VoteTabulationTests
{
    [Fact]
    public void VotingResultsShouldTabulateVotesForSinglePlayer()
    {
        // Arrange
        Player a = new("A", new RandomController(new ClaimStartingRoleStrategy()));
        Player b = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player c = new("C", new RandomController(new ClaimStartingRoleStrategy()));
        Dictionary<Player, Player> votes = new()
        {
            [a] = b,
            [b] = c,
            [c] = b
        };

        // Act
        IDictionary<Player, int> results = VotingHelper.GetVotingResults(votes);
        
        // Assert
        results.ShouldNotBeNull();
        results.Keys.Count.ShouldBe(3);
        results[a].ShouldBe(0);
        results[b].ShouldBe(2);
        results[c].ShouldBe(1);
    }

    [Fact]
    public void VotingResultsShouldTabulateCircleVote()
    {
        // Arrange
        Player a = new("A", new RandomController(new ClaimStartingRoleStrategy()));
        Player b = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player c = new("C", new RandomController(new ClaimStartingRoleStrategy()));
        Dictionary<Player, Player> votes = new()
        {
            [a] = b,
            [b] = c,
            [c] = a
        };

        // Act
        var results = VotingHelper.GetVotingResults(votes);
        
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
        Player a = new("A", new RandomController(new ClaimStartingRoleStrategy()));
        Player b = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player c = new("C", new RandomController(new ClaimStartingRoleStrategy()));
        Player d = new("D", new RandomController(new ClaimStartingRoleStrategy()));
        Dictionary<Player, Player> votes = new()
        {
            [a] = b,
            [b] = c,
            [c] = b,
            [d] = c
        };

        // Act
        var results = VotingHelper.GetVotingResults(votes);
        
        // Assert
        results.ShouldNotBeNull();
        results.Keys.Count.ShouldBe(4);
        results[a].ShouldBe(0);
        results[b].ShouldBe(2);
        results[c].ShouldBe(2);
        results[d].ShouldBe(0);
    }
}