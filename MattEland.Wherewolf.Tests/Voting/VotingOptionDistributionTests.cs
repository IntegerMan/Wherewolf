using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests.Voting;

public class VotingOptionDistributionTests
{
    [Fact]
    public void VotingOptionsWith3PlayersShouldHaveCorrectPermutationCount()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("B", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("C", new RandomController(new ClaimStartingRoleStrategy())));

        // Act
        var permutations = setup.GetVotingPermutations().ToList();

        // Assert
        permutations.Count.ShouldBe(8);
    }
    
    [Fact]
    public void VotingOptionsWith4PlayersShouldHaveCorrectPermutationCount()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("B", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("C", new RandomController(new ClaimStartingRoleStrategy())),
            new Player("D", new RandomController(new ClaimStartingRoleStrategy())));

        // Act
        var permutations = setup.GetVotingPermutations().ToList();

        // Assert
        permutations.Count.ShouldBe(81);
    }    
    
    [Fact]
    public void VotingOptionsWith5PlayersShouldHaveCorrectPermutationCount()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("B", new RandomController(new ClaimStartingRoleStrategy())), 
            new Player("C", new RandomController(new ClaimStartingRoleStrategy())),
            new Player("D", new RandomController(new ClaimStartingRoleStrategy())),
            new Player("E", new RandomController(new ClaimStartingRoleStrategy())));

        // Act
        var permutations = setup.GetVotingPermutations().ToList();

        // Assert
        permutations.Count.ShouldBe(1024);
    }    
}