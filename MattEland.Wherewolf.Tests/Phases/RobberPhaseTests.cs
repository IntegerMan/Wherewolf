using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Phases;

public class RobberPhaseTests
{
    [Fact]
    public void RobberPhaseShouldGeneratePermutationsWhereRobberHasRobbed()
    {
        // Arrange
        RobberNightPhase phase = new();
        Player[] players = [new("Robber", new RandomController(new ClaimStartingRoleStrategy())), new("WW", new RandomController()), new("Villager", new RandomController())];
        GameRole[] roles =
        [
            GameRole.Robber, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager,
            GameRole.Werewolf
        ];
        
        GameSetup setup = new();
        setup.SetPlayers(players.ToArray());
        setup.AddRoles(roles.ToArray());
        GameState state = new GameState(setup, roles, support: 1);

        // Act
        List<GameState> possibleStates = phase.BuildPossibleStates(state).ToList();

        // Assert
        possibleStates.Count.ShouldBe(2);
        possibleStates.Count(p => p["Robber"].Role != GameRole.Robber).ShouldBe(2);
    }
    
    [Fact]
    public void RobberPhaseShouldGeneratePermutationsWhereParentStateHasDifferentStartAndEndRoles()
    {
        // Arrange
        RobberNightPhase phase = new();
        Player[] players = [
            new("Robber", new RandomController(new ClaimStartingRoleStrategy())), 
            new("WW", new RandomController(new ClaimStartingRoleStrategy())), 
            new("Villager", new RandomController(new ClaimStartingRoleStrategy()))
        ];
        GameRole[] roles =
        [
            GameRole.Robber, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager,
            GameRole.Werewolf
        ];
        GameSetup setup = new();
        setup.SetPlayers(players.ToArray());
        setup.AddRoles(roles.ToArray());
        GameState state = new GameState(setup, roles, support: 1);

        // Act
        List<GameState> newStates = phase.BuildPossibleStates(state).ToList();

        // Assert
        newStates.ShouldAllBe(s => s["Robber"].Role != GameRole.Robber);
    }
}