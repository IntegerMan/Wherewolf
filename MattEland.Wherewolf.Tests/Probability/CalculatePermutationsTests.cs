using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Probability;

public class CalculatePermutationsTests
{
    [Fact]
    public void EachPhaseShouldHaveMoreTotalPermutationsThanTheLast()
    {
        // Arrange
        GameSetup setup = CreateGameSetup();
        setup.StartGame();
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
        setup.StartGame();

        // Act
        IEnumerable<GameState> permutations = setup.GetPermutationsAtPhase(null); // voting
        permutations = permutations.Where(p => p.GetStartRole("A") == GameRole.Robber);
        
        // Assert
        permutations.ShouldAllBe(p => p["A"].Role != GameRole.Robber, "Permutations existed where the robber started the robber phase as robber and ended as the robber");
        permutations.ShouldAllBe(p => p.PlayerSlots.Count(p => p.Role == GameRole.Robber) == 1);
    }

    private static GameSetup CreateGameSetup()
    {
        GameSetup setup = new();
        setup.AddRole(GameRole.Robber);
        setup.AddRole(GameRole.Werewolf);
        setup.AddRole(GameRole.Villager, 3);
        setup.AddRole(GameRole.Werewolf);
        Player robberPlayer = new("A", new RandomController());
        setup.SetPlayers(
            robberPlayer,
            new Player("B", new RandomController()),
            new Player("C", new RandomController())
        );
        return setup;
    }
}