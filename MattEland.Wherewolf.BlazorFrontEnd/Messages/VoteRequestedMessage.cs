using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.BlazorFrontEnd.Messages;

public record VoteRequestedMessage(GameState State, PlayerProbabilities PlayerProbabilities, Dictionary<Player, double> VictoryProbabilities);