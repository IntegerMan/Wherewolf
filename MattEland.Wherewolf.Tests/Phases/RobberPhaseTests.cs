using MattEland.Wherewolf.Controllers;
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
        GameSetup setup = new GameSetup();
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(GameRole.Robber, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager);
        GameState state = setup.StartGame(new NonShuffler());

        // Act
        List<GameState> possibleStates = phase.BuildPossibleStates(state).ToList();

        // Assert
        possibleStates.Count.ShouldBe(2);
        possibleStates.Count(p => p.GetPlayerSlot(state.Players.First()).EndOfPhaseRole != GameRole.Robber).ShouldBe(2);
    }
}