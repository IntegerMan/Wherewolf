using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy(Random rand) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        KeyValuePair<GameRole, VoteStatistics>[] roleClaimVoteStats =
            VotingHelper.GetRoleClaimVoteStatistics(player, gameState).ToArray();

        double best = roleClaimVoteStats.Min(rs => rs.Value.VotesPerGame);
            
        // Consider the top role based on the lowest win % for our opponents voting for who they think we are
        // This will encourage us to claim the role that is least likely to get us killed
        GameRole[] bestRoles = roleClaimVoteStats
            .Where(rs => rs.Value.VotesPerGame <= best)
            .OrderByDescending(rs => rs.Value.Support)
            .ThenBy(_ => rand.Next())
            .Select(rs => rs.Key)
            .ToArray();

        // If there's only one role, we're done
        if (bestRoles.Length == 1)
        {
            return bestRoles[0];
        }

        // If we have a tie, we want to go with the role we actually started as
        GameRole startRole = gameState.GetStartRole(player);
        if (bestRoles.Contains(startRole))
        {
            return startRole;
        }

        return bestRoles.First();
    }
}