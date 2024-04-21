using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests.Probability;

public class BasicProbabilityTests : GameTestsBase
{
    [Fact]
    public void PlayersShouldBeCertainOfTheirStartRole()
    {
        // Arrange
        GameSetup gameSetup = new GameSetup();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player player = gameSetup.Players.First();
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();
        PlayerState playerState = state.GetPlayerStates(player);

        // Act
        PlayerProbabilities probabilities = playerState.Probabilities;
        SlotRoleProbabilities slotProbabilities = probabilities.GetSlotProbabilities(state.GetSlot(player.Name));
        
        // Assert
        slotProbabilities["Werewolf"].ShouldBe(1);
        slotProbabilities["Villager"].ShouldBe(0);
    }
    
    [Fact]
    public void PlayersShouldHaveAccuratePercentagesForOtherPlayerStartingRolesOnNoInformation()
    {
        // Arrange
        GameSetup gameSetup = new GameSetup();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player villager = gameSetup.Players.ToList()[2]; // This is a villager
        Player werewolf = gameSetup.Players.ToList()[0];
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerState playerState = state.GetPlayerStates(villager);
        PlayerProbabilities probabilities = playerState.Probabilities;
        SlotRoleProbabilities slotProbabilities = probabilities.GetSlotProbabilities(state.GetSlot(werewolf.Name));

        // Assert
        // Calculating probabilities of roles based on remaining roles since the villager knows they're a villager
        slotProbabilities["Werewolf"].ShouldBe(2/5d);
        slotProbabilities["Villager"].ShouldBe(3/5d);
    }
    
    [Fact]
    public void WerewolvesShouldBeCertainNonWolvesAreVillagersInWolvesVsVillagers()
    {
        // Arrange
        GameSetup gameSetup = new GameSetup();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player villager = gameSetup.Players.ToList()[2]; // This is a villager
        Player werewolf = gameSetup.Players.ToList()[0];
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerState playerState = state.GetPlayerStates(werewolf);
        PlayerProbabilities probabilities = playerState.Probabilities;
        SlotRoleProbabilities slotProbabilities = probabilities.GetSlotProbabilities(state.GetSlot(villager.Name));

        // Assert
        slotProbabilities["Werewolf"].ShouldBe(0);
        slotProbabilities["Villager"].ShouldBe(1);
    }
    
    [Fact]
    public void WerewolvesShouldBeCertainFellowWolvesAreWolves()
    {
        // Arrange
        GameSetup gameSetup = new GameSetup();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player ww1 = gameSetup.Players.ToList()[0];
        Player ww2 = gameSetup.Players.ToList()[1];
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerState playerState = state.GetPlayerStates(ww1);
        PlayerProbabilities probabilities = playerState.Probabilities;
        SlotRoleProbabilities slotProbabilities = probabilities.GetSlotProbabilities(state.GetSlot(ww2.Name));

        // Assert
        slotProbabilities["Werewolf"].ShouldBe(1);
        slotProbabilities["Villager"].ShouldBe(0);
    }
}