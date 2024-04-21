using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Tests.Probability;

public class GameSetupPermutationTests
{
    [Fact]
    public void GameSetupShouldBuildValidPermutations()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(new VillagerRole(), new VillagerRole(), new WerewolfRole(), new VillagerRole(), new VillagerRole(), new WerewolfRole());
        
        // Act
        List<GamePermutation> permutations = setup.Permutations.ToList();
        
        // Assert
        permutations.ShouldNotBeEmpty();
        permutations.Count.ShouldBe(15); // Unique combinations of 2 WW's and 4 villagers
        permutations.Sum(p => p.Support).ShouldBe(720); // Total possible game states including identical states for different WW / Villager cards. This is calculated as number of cards factorial, which is 6 in this case
    }
}