using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class RandomOptimalVoteController : RandomController
{
    public RandomOptimalVoteController(IRoleClaimStrategy? roleClaimStrategy = null, Random? rand = null) 
        : base(roleClaimStrategy, rand)
    {
    }

    public override Player GetPlayerVote(Player votingPlayer, GameState state)
    {
        // TODO: This doesn't take claims into account
        Dictionary<Player, float> bestProbabilities = VotingHelper.GetVoteVictoryProbabilities(votingPlayer, state);

        float maxProb = bestProbabilities.Values.Max();
        
        return bestProbabilities
            .OrderBy(_ => Rand.Next())
            .First(kvp => Math.Abs(kvp.Value - maxProb) < double.Epsilon).Key;
    }
}