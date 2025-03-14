using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Voting;

public class VoteWinProbabilityTests
{
    [Fact]
    public void VoteProbabilitiesShouldBeEqualWhenNoInformationAvailable()
    {
        // Arrange
        RandomController controller = new RandomController(new ClaimFixedRoleStrategy(GameRole.Villager, (p,_) => new VillagerNoActionClaim(p)));
        Player a = new("A", controller); // Villager
        Player b = new("B", controller); // Villager
        Player c = new("C", controller); // Werewolf
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 2);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 2);
        GameManager game = new(setup);
        game.RunToEnd();
        
        // Act
        var probabilities = VotingHelper.GetVoteVictoryProbabilities(a, game.CurrentState);
        
        // Assert
        probabilities.Keys.Count.ShouldBe(2);
        probabilities[b].ShouldBe(probabilities[c]); // because we know nothing, they should have equal probability
    }
    
    [Fact]
    public void VoteProbabilitiesShouldBeBiasedWithFullInformationAvailable()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 4);
        GameManager game = new(setup);
        game.RunToEnd();
        
        // Act
        var probabilities = VotingHelper.GetVoteVictoryProbabilities(a, game.CurrentState);
        
        // Assert
        probabilities.Keys.Count.ShouldBe(2);
        probabilities[c].ShouldBeGreaterThan(probabilities[b]); // WW's win more if the villager dies
    }
}