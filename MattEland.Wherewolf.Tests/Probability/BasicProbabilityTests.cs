using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Tests.Probability;

public class BasicProbabilityTests : GameTestsBase
{
    [Fact]
    public void PlayersShouldBeCertainOfTheirStartRole()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player player = gameSetup.Players.First();
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerProbabilities probabilities = state.CalculateProbabilities(player);
        SlotRoleProbabilities slotProbabilities = probabilities.GetCurrentProbabilities(state.GetSlot(player.Name));
        
        // Assert
        slotProbabilities[GameRole.Werewolf].Probability.ShouldBe(1);
        slotProbabilities[GameRole.Villager].Probability.ShouldBe(0);
    }
    
    [Fact]
    public void PlayersShouldHaveAccuratePercentagesForOtherPlayerStartingRolesOnNoInformation()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player villager = gameSetup.Players.ToList()[2]; // This is a villager
        Player werewolf = gameSetup.Players.ToList()[0];
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerProbabilities probabilities = state.CalculateProbabilities(villager);
        SlotRoleProbabilities slotProbabilities = probabilities.GetCurrentProbabilities(state.GetSlot(werewolf.Name));

        // Assert
        // Calculating probabilities of roles based on remaining roles since the villager knows they're a villager
        slotProbabilities[GameRole.Werewolf].Probability.ShouldBe(2/5d);
        slotProbabilities[GameRole.Villager].Probability.ShouldBe(3/5d);
    }
    
    [Fact]
    public void WerewolvesShouldBeCertainNonWolvesAreVillagersInWolvesVsVillagers()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player villager = gameSetup.Players.ToList()[2]; // This is a villager
        Player werewolf = gameSetup.Players.ToList()[0];
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerProbabilities probabilities = state.CalculateProbabilities(werewolf);
        SlotRoleProbabilities slotProbabilities = probabilities.GetCurrentProbabilities(state.GetSlot(villager.Name));

        // Assert
        slotProbabilities[GameRole.Werewolf].Probability.ShouldBe(0);
        slotProbabilities[GameRole.Villager].Probability.ShouldBe(1);
    }
    
    [Fact]
    public void WerewolvesShouldBeCertainFellowWolvesAreWolves()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player ww1 = gameSetup.Players.ToList()[0];
        Player ww2 = gameSetup.Players.ToList()[1];
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerProbabilities probabilities = state.CalculateProbabilities(ww1);
        SlotRoleProbabilities slotProbabilities = probabilities.GetCurrentProbabilities(state.GetSlot(ww2.Name));

        // Assert
        slotProbabilities.Role[GameRole.Werewolf].Probability.ShouldBe(1);
        slotProbabilities.Role[GameRole.Villager].Probability.ShouldBe(0);
    }
    
    [Fact]
    public void PlayersShouldBeCertainOfTheirEndRoleWhenNoRoleChangersExist()
    {
        // Arrange
        GameSetup gameSetup = new();
        AddMinimumRequiredPlayers(gameSetup);
        AddMinimumRequiredRoles(gameSetup);
        Player player = gameSetup.Players.First();
        GameState state = gameSetup.StartGame(new NonShuffler()).RunToEnd();

        // Act
        PlayerProbabilities probabilities = state.CalculateProbabilities(player);
        SlotRoleProbabilities slotProbabilities = probabilities.GetCurrentProbabilities(state.GetSlot(player.Name));
        
        // Assert
        slotProbabilities.Count.ShouldBe(gameSetup.Roles.Distinct().Count());
        slotProbabilities[GameRole.Werewolf].Probability.ShouldBe(1);
        slotProbabilities[GameRole.Villager].Probability.ShouldBe(0);
    }
}