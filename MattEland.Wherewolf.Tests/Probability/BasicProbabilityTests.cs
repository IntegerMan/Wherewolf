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
}