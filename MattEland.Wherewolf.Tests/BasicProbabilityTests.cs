using MattEland.Wherewolf.Tests.Helpers;

namespace MattEland.Wherewolf.Tests;

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
        List<CardProbability> probabilities = playerState.CalculateStartingCardProbabilities(player).ToList();

        // Assert
        probabilities.Count.ShouldBe(1);
        probabilities[0].Card.ShouldBe(state.GetPlayerSlot(player).StartRole);
        probabilities[0].Probability.ShouldBe(1);
    }
}