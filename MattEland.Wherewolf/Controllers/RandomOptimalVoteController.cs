using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Controllers;

public class RandomOptimalVoteController(IRoleClaimStrategy? roleClaimStrategy = null, Random? rand = null)
    : RandomController(roleClaimStrategy, rand)
{
    public override Player GetPlayerVote(Player votingPlayer, GameState state)
    {
        Dictionary<Player, double> bestProbabilities = VotingHelper.GetVoteVictoryProbabilities(votingPlayer, state);

        double maxProb = bestProbabilities.Values.Max();
        
        return bestProbabilities
            .OrderBy(_ => Rand.Next())
            .First(kvp => Math.Abs(kvp.Value - maxProb) < double.Epsilon).Key;
    }
}