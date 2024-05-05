using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Phases;

public class RobberPhaseTests
{
    [Fact]
    public void RobberPhaseShouldGeneratePermutationsWhereRobberHasRobbed()
    {
        // Arrange
        RobberNightPhase phase = new();
        Player[] players = [new("Robber", new RandomController()), new("WW", new RandomController()), new("Villager", new RandomController())];
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
        possibleStates.Count(p => p["Robber"].EndOfPhaseRole != GameRole.Robber).ShouldBe(2);
    }
    
    [Fact]
    public void RobberPhaseShouldGeneratePermutationsWhereParentStateHasDifferentStartAndEndRoles()
    {
        // Arrange
        RobberNightPhase phase = new();
        Player[] players = [
            new("Robber", new RandomController()), 
            new("WW", new RandomController()), 
            new("Villager", new RandomController())
        ];
        GameRole[] roles =
        [
            GameRole.Robber, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager,
            GameRole.Werewolf
        ];
        GameEvent[] priorEvents = [];
        GamePhase[] remainingPhases = [new RobberNightPhase()];
        GameState state = new(players, roles, remainingPhases, priorEvents);

        // Act
        List<GameState?> newStates = phase.BuildPossibleStates(state)
            .Select(p => p.Parent)
            .ToList();

        // Assert
        newStates.ShouldAllBe(s => s != null);
        newStates.ShouldAllBe(s => s.CurrentPhase!.Name == "Robber");
        newStates.ShouldAllBe(s => s["Robber"].StartRole == GameRole.Robber);
        newStates.ShouldAllBe(s => s["Robber"].EndOfPhaseRole != GameRole.Robber);
    }
}