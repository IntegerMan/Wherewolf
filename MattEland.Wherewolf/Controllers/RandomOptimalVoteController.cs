using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Controllers;

public class RandomOptimalVoteController(IRoleClaimStrategy? roleClaimStrategy = null, Random? rand = null)
    : RandomController(roleClaimStrategy, rand)
{
    public override void GetPlayerVote(Player votingPlayer, GameState state, Action<Player> callback)
    {
        Dictionary<Player, double> bestProbabilities = VotingHelper.GetVoteVictoryProbabilities(votingPlayer, state);

        double maxProb = bestProbabilities.Values.Max();
        
        Player choice = bestProbabilities
            .OrderBy(_ => Rand.Next())
            .First(kvp => Math.Abs(kvp.Value - maxProb) < double.Epsilon)
            .Key;
        callback(choice);
    }
}