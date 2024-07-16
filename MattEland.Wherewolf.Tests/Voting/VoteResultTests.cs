using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests.Voting;

public class VoteResultTests
{
    [Fact]
    public void SinglePlayerVotedOutShouldBeDead()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        
        GameSetup setup = new();
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.StartGame(new NonShuffler()).RunToEnd();
        Dictionary<Player, int> votes = new()
        {
            [a] = 1,
            [b] = 2,
            [c] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadPlayers.Count().ShouldBe(1);
        results.DeadPlayers.ShouldContain(b);
    }
    
    [Fact]
    public void TwoPlayersVotedOutInTieShouldBeDead()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        Player d = new("D", new RandomController());
        
        GameSetup setup = new();
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Villager, 5);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.StartGame(new NonShuffler()).RunToEnd();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 2,
            [c] = 2,
            [d] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadPlayers.Count().ShouldBe(2);
        results.DeadPlayers.ShouldContain(b);
        results.DeadPlayers.ShouldContain(c);
    }    
    
    [Fact]
    public void AllSkippingShouldResultInNoneDead()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        
        GameSetup setup = new();
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.StartGame(new NonShuffler()).RunToEnd();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadPlayers.ShouldBeEmpty();
    }    
    
    [Fact]
    public void AllCircleVotingShouldResultInNoneDead()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        
        GameSetup setup = new();
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.StartGame(new NonShuffler()).RunToEnd();
        Dictionary<Player, int> votes = new()
        {
            [a] = 1,
            [b] = 1,
            [c] = 1
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadPlayers.ShouldBeEmpty();
    }    
    
    [Fact]
    public void AllButOneSkippingShouldResultInOneDead()
    {
        // Arrange
        Player a = new("A", new RandomController());
        Player b = new("B", new RandomController());
        Player c = new("C", new RandomController());
        
        GameSetup setup = new();
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.StartGame(new NonShuffler()).RunToEnd();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 1
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadPlayers.Count().ShouldBe(1);
        results.DeadPlayers.ShouldContain(c);
    }
}