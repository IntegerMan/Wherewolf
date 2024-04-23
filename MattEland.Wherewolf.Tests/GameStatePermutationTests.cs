using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests;

public class GameStatePermutationTests
{
    [Fact]
    public void TwoWolvesGameStateAtWerewolfPhaseShouldHaveOneChildStateForNextPhase()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(new WerewolfRole(), new WerewolfRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole());
        GameState startState = setup.StartGame(new NonShuffler());

        // Act
        List<GameState> childStates = startState.PossibleNextStates.ToList();

        // Assert
        childStates.ShouldNotBeNull();
        childStates.Count.ShouldBe(1);
        childStates.Single().CurrentPhase.ShouldNotBe(startState.CurrentPhase);
    }

    [Fact]
    public void ChildStatesShouldHaveCorrectParentState()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(new WerewolfRole(), new WerewolfRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole());
        GameState startState = setup.StartGame(new NonShuffler());

        // Act
        GameState? childState = startState.PossibleNextStates.FirstOrDefault();

        // Assert
        childState.ShouldNotBeNull();
        childState.Parent.ShouldBe(startState);
    }
    
    [Fact]
    public void RootStateShouldHaveNoParent()
    {
        // Arrange
        GameSetup setup = new();
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(new WerewolfRole(), new WerewolfRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole());

        // Act
        GameState startState = setup.StartGame(new NonShuffler());

        // Assert
        startState.Parent.ShouldBeNull();
    }    
}