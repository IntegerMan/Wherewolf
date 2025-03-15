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
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
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
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Villager, 5);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
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
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
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
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
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
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c);
        setup.AddRole(GameRole.Villager, 4);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
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
        
    [Fact]
    public void KillingAPlayerShouldResultInTheirRoleDead()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 5);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 3,
            [d] = 1
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadRoles.Count().ShouldBe(1);
        results.DeadRoles.ShouldContain(GameRole.Villager);
    }
    
        
    [Fact]
    public void KillingTwoPlayersShouldResultInTheirRolesDead()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 5);

        GameState state = setup.RunGame();
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
        results.DeadRoles.Count().ShouldBe(2);
        results.DeadRoles.ShouldContain(GameRole.Villager);
        results.DeadRoles.ShouldContain(GameRole.Werewolf);
    }
    
       
    [Fact]
    public void KillingTwoIdenticalPlayersShouldResultInOneRoleDead()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 5);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 2,
            [d] = 2
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadRoles.Count().ShouldBe(1);
        results.DeadRoles.ShouldContain(GameRole.Villager);
    }       
    
    [Fact]
    public void KillingNoPlayersShouldResultInNoRolesDead()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 5);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 0,
            [d] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.DeadRoles.ShouldBeEmpty();
    }
    
    [Fact]
    public void WinningPlayersShouldIncludeEvilForNoWerewolfDead()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 5);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 0,
            [d] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.WinningTeam.ShouldBe(Team.Werewolf);
        results.WinningPlayers.ShouldContain(a);
        results.WinningPlayers.ShouldContain(b);
        results.LosingPlayers.ShouldContain(c);
        results.LosingPlayers.ShouldContain(d);
    }
    
    [Fact]
    public void WinningPlayersShouldIncludeGoodForWerewolfDead()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Werewolf
        Player b = new("B", new RandomController()); // Werewolf
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Werewolf, 2);
        setup.AddRole(GameRole.Villager, 5);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 2, 
            [b] = 0,
            [c] = 2, // will be dead, but still wins since villager team
            [d] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.WinningTeam.ShouldBe(Team.Villager);
        results.LosingPlayers.ShouldContain(a);
        results.LosingPlayers.ShouldContain(b);
        results.WinningPlayers.ShouldContain(c);
        results.WinningPlayers.ShouldContain(d);
    }    
    
    [Fact]
    public void WinningPlayersShouldIncludeGoodForSkipOnNoEvils()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Villager
        Player b = new("B", new RandomController()); // Villager
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Villager, 5);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 0,
            [b] = 0,
            [c] = 0,
            [d] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.WinningTeam.ShouldBe(Team.Villager);
        results.WinningPlayers.ShouldContain(a);
        results.WinningPlayers.ShouldContain(b);
        results.WinningPlayers.ShouldContain(c);
        results.WinningPlayers.ShouldContain(d);
        results.LosingPlayers.ShouldBeEmpty();
    }
    
    [Fact]
    public void WinningPlayersBeEmptyForAllGoodWithExecution()
    {
        // Arrange
        Player a = new("A", new RandomController()); // Villager
        Player b = new("B", new RandomController()); // Villager
        Player c = new("C", new RandomController()); // Villager
        Player d = new("D", new RandomController()); // Villager
        
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(a, b, c, d);
        setup.AddRole(GameRole.Villager, 5);
        setup.AddRole(GameRole.Werewolf, 2);

        GameState state = setup.RunGame();
        Dictionary<Player, int> votes = new()
        {
            [a] = 3,
            [b] = 1,
            [c] = 0,
            [d] = 0
        };
        
        // Act
        var results = state.DetermineGameResults(votes);

        // Assert
        results.WinningTeam.ShouldBe(Team.Werewolf);
        results.WinningPlayers.ShouldBeEmpty();
        results.LosingPlayers.ShouldContain(a);
        results.LosingPlayers.ShouldContain(b);
        results.LosingPlayers.ShouldContain(c);
        results.LosingPlayers.ShouldContain(d);
    }           
}