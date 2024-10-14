using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Probability;

public class GameSetupPermutationTests
{
    [Fact]
    public void GameSetupShouldBuildValidPermutations()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(GameRole.Villager, GameRole.Villager, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Werewolf);
        
        // Act
        List<GameState> permutations = setup.GetPermutationsAtPhase(setup.Phases.FirstOrDefault()).ToList();
        
        // Assert
        permutations.ShouldNotBeEmpty();
        permutations.Count.ShouldBe(15); // Unique combinations of 2 WW's and 4 villagers
        permutations.Sum(p => p.Support).ShouldBe(720); // Total possible game states including identical states for different WW / Villager cards. This is calculated as number of cards factorial, which is 6 in this case
    }
}