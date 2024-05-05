using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests.Probability;

public class CalculatePermutationsTests
{
    [Fact]
    public void EachPhaseShouldHaveMoreTotalPermutationsThanTheLast()
    {
        // Arrange
        GameSetup setup = CreateGameSetup();
        List<GamePhase?> phases = setup.Phases.ToList()!;
        phases.Add(null); // Voting Phase

        // Act / Assert
        int lastCount = 0;
        foreach (var phase in phases)
        {
            IEnumerable<GameState> perms = setup.GetPermutationsAtPhase(phase);
            int count = perms.Count();
            count.ShouldBeGreaterThanOrEqualTo(lastCount);
            lastCount = count;
        }
    }
    
    [Fact]
    public void RobberPermutationShouldHaveTwoPlayersChangeRolesInEveryPermutation()
    {
        // Arrange
        GameSetup setup = CreateGameSetup();
        RobberNightPhase robberNightPhase = setup.Phases.OfType<RobberNightPhase>().First();

        // Act
        IEnumerable<GameState> permutations = setup.GetPermutationsAtPhase(robberNightPhase);
        
        // Filter down to only cases where we have a robber in player 1's slot
        permutations = permutations.Where(p => p["A"].StartRole == GameRole.Robber);
        
        // Assert
        permutations.ShouldNotBeEmpty();
        permutations.ShouldAllBe(p => p["A"].EndOfPhaseRole != GameRole.Robber, "Permutations existed where the robber started the robber phase as robber and ended as the robber");
    }

    private static GameSetup CreateGameSetup()
    {
        GameSetup setup = new();
        setup.AddRole(GameRole.Robber);
        setup.AddRole(GameRole.Werewolf);
        setup.AddRole(GameRole.Villager, 3);
        setup.AddRole(GameRole.Werewolf);
        Player robberPlayer = new("A", new RandomController());
        setup.AddPlayers(
            robberPlayer,
            new Player("B", new RandomController()),
            new Player("C", new RandomController())
        );
        return setup;
    }
}