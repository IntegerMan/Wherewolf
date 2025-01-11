using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

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
        GameEvent[] priorEvents = [];
        GamePhase[] remainingPhases = [new RobberNightPhase()];
        GameState state = new(players, roles, remainingPhases, priorEvents);

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
        GameEvent[] priorEvents = [];
        GamePhase[] remainingPhases = [];
        GameState state = new(players, roles, remainingPhases, priorEvents);

        // Act
        List<GameState> newStates = phase.BuildPossibleStates(state).ToList();

        // Assert
        newStates.ShouldAllBe(s => s["Robber"].Role != GameRole.Robber);
    }
}