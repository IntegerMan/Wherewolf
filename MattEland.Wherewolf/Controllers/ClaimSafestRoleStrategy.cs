using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy(Random rand) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(player);
        Team startTeam = startRole.GetTeam();
        
        KeyValuePair<GameRole, VoteStatistics>[] roleClaimVoteStats =
            VotingHelper.GetRoleClaimVoteStatistics(player, gameState).ToArray();

        double best = roleClaimVoteStats.Min(rs => rs.Value.VoteFactor);
            
        // Consider the top role based on the lowest win % for our opponents voting for who they think we are
        // This will encourage us to claim the role that is least likely to get us killed
        GameRole[] bestRoles = roleClaimVoteStats
            .Where(rs => rs.Value.VoteFactor <= best)
            // This makes the werewolf team more likely to claim safe roles while villager team more likely to claim their own role
            .OrderByDescending(rs => startTeam == Team.Werewolf ? rs.Value.OutOfPlayPercent : rs.Value.InPlayPercent)
            // High support are more likely to be true
            .ThenByDescending(rs => rs.Value.Support)
            // Low claims are good
            .ThenBy(rs => rs.Value.OtherClaims)
            // Resolve ties with randomness
            .ThenBy(_ => rand.Next())
            .Select(rs => rs.Key)
            .ToArray();

        // If there's only one role, we're done
        if (bestRoles.Length == 1)
        {
            return bestRoles[0];
        }

        // If we have a tie, we want to go with the role we actually started as
        if (bestRoles.Contains(startRole))
        {
            return startRole;
        }

        return bestRoles.First();
    }

    public SpecificRoleClaim GetSpecificRoleClaim(Player player, GameState gameState, SpecificRoleClaim[] possibleClaims,
        GameRole initialClaim)
    {
        KeyValuePair<SpecificRoleClaim, VoteStatistics>[] options = 
            VotingHelper.GetSpecificRoleClaimVoteStatistics(player, gameState, possibleClaims)
                .Where(o => o.Value.Support > 0)
                .OrderBy(o => o.Value.VoteFactor)
                .ToArray();
        
        double best = options.Min(rs => rs.Value.VoteFactor);
        SpecificRoleClaim[] rankedChoices = options
            .Where(rs => rs.Value.VoteFactor <= best)
            // Stick with our initial role claim if possible
            .OrderByDescending(rs => rs.Key.Role == initialClaim)
            // This makes the werewolf team more likely to claim safe roles while villager team more likely to claim their own role
            .ThenByDescending(rs => rs.Value.InPlayPercent)
            // High support are more likely to be true
            .ThenByDescending(rs => rs.Value.Support)
            // Low claims are good
            .ThenBy(rs => rs.Value.OtherClaims)
            // Resolve ties with randomness
            .ThenBy(_ => rand.Next())
            .Select(o => o.Key)
            .ToArray();

        return rankedChoices.FirstOrDefault() ?? options.First().Key;
    }
}