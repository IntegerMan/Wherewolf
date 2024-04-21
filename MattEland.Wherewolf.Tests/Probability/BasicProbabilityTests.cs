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

        // Act
        PlayerState playerState = state.GetPlayerStates(player);
        IDictionary<string, double> probabilities = playerState.CalculateStartingCardProbabilities(player);

        // Assert
        probabilities["Werewolf"].ShouldBe(1);
        probabilities["Villager"].ShouldBe(0);
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
        IDictionary<string, double> probabilities = playerState.CalculateStartingCardProbabilities(werewolf);

        // Assert
        probabilities.Count.ShouldBe(2);
        // Calculating probabilities of roles based on remaining roles since the villager knows they're a villager
        probabilities["Werewolf"].ShouldBe(2/5d);
        probabilities["Villager"].ShouldBe(3/5d);
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
        IDictionary<string, double> probabilities = playerState.CalculateStartingCardProbabilities(villager);

        // Assert
        probabilities["Werewolf"].ShouldBe(0);
        probabilities["Villager"].ShouldBe(1);
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
        IDictionary<string, double> probabilities = playerState.CalculateStartingCardProbabilities(ww2);

        // Assert
        probabilities["Werewolf"].ShouldBe(1);
        probabilities["Villager"].ShouldBe(0);
    }
}