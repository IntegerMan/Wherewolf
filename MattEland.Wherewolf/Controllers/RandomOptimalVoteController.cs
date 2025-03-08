using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Controllers;

public class RandomOptimalVoteController(IRoleClaimStrategy? roleClaimStrategy = null, Random? rand = null)
    : RandomController(roleClaimStrategy, rand)
{
    public override void GetPlayerVote(Player votingPlayer, GameState state, PlayerProbabilities probabilities, Dictionary<Player, double> voteProbabilities, Action<Player> callback)
    {
        double maxProb = voteProbabilities.Values.Max();
        Player choice = voteProbabilities
            .OrderBy(_ => Rand.Next())
            .First(kvp => Math.Abs(kvp.Value - maxProb) < double.Epsilon)
            .Key;
        callback(choice);
    }
}