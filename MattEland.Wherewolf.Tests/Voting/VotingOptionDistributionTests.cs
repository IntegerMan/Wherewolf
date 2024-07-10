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
        setup.AddPlayers(new Player("A", new RandomController()), 
            new Player("B", new RandomController()), 
            new Player("C", new RandomController()));

        // Act
        var permutations = setup.GetVotingPermutations().ToList();

        // Assert
        permutations.Count.ShouldBe(27);
    }
    
    [Fact]
    public void VotingOptionsWith4PlayersShouldHaveCorrectPermutationCount()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), 
            new Player("B", new RandomController()), 
            new Player("C", new RandomController()),
            new Player("D", new RandomController()));

        // Act
        var permutations = setup.GetVotingPermutations().ToList();

        // Assert
        permutations.Count.ShouldBe(256);
    }    
    
    [Fact]
    public void VotingOptionsWith5PlayersShouldHaveCorrectPermutationCount()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), 
            new Player("B", new RandomController()), 
            new Player("C", new RandomController()),
            new Player("D", new RandomController()),
            new Player("E", new RandomController()));

        // Act
        var permutations = setup.GetVotingPermutations().ToList();

        // Assert
        permutations.Count.ShouldBe(3125);
    }    
}