using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests;

public class GameStatePermutationTests
{
    [Fact]
    public void TwoWolvesGameStateAtWerewolfPhaseShouldHaveOneChildStateForNextPhase()
    {
        // Arrange
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(GameRole.Werewolf, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager);
        GameState startState = setup.StartGame();

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
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()),
            new Player("C", new RandomController()));
        setup.AddRoles(GameRole.Werewolf, GameRole.Werewolf, GameRole.Villager, GameRole.Villager,
            GameRole.Villager, GameRole.Villager);
        GameState startState = setup.StartGame();

        // Act
        GameState? childState = startState.PossibleNextStates.FirstOrDefault();

        // Assert
        childState.ShouldNotBeNull();
        childState.Parent.ShouldBe(startState);
    }

    [Fact]
    public void ChildStatesShouldHaveDifferentSlotArrays()
    {
        // Arrange
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(GameRole.Werewolf, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager);
        GameState startState = setup.StartGame();

        // Act
        GameState? childState = startState.PossibleNextStates.FirstOrDefault();

        // Assert
        childState.ShouldNotBeNull();
        childState.CenterSlots.ShouldNotBe(startState.CenterSlots);
        childState.PlayerSlots.ShouldNotBe(startState.PlayerSlots);
    }

    [Fact]
    public void ChildStatesShouldHaveDifferentSlotObjects()
    {
        // Arrange
        GameSetup setup = new(new NonShuffler());
        setup.AddPlayers(new Player("A", new RandomController()), new Player("B", new RandomController()), new Player("C", new RandomController()));
        setup.AddRoles(GameRole.Werewolf, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager);
        GameState startState = setup.StartGame();

        // Act
        GameState? childState = startState.PossibleNextStates.FirstOrDefault();

        // Assert
        childState.ShouldNotBeNull();
        foreach (var slot in childState.AllSlots)
        {
            startState.PlayerSlots.ShouldNotContain(slot);
            startState.CenterSlots.ShouldNotContain(slot);
            startState.AllSlots.ShouldNotContain(slot);
        }
    }    
}