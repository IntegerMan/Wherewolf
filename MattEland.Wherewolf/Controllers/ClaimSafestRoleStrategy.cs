using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy(Random rand) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(player);
        GameState[] possibleEndStates = gameState.Setup.GetPermutationsAtPhase(null).ToArray();

        // Now, let's take roles into account and see what victory % each other player gets voting for us based on who they think we are
        Dictionary<GameRole, VoteStatistics> roleStats 
            = GetOtherPlayersWinRatesGivenMyRoleClaim(player, gameState, possibleEndStates);

        // Now let's consider the top role based on the lowest win % for our opponents voting for who they think we are
        // This will encourage us to claim the role that is least likely to get us killed
        double best = roleStats.Values.Min(rs => rs.VotesPerGame);
        GameRole[] bestRoles = roleStats
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
        if (bestRoles.Contains(startRole))
        {
            return startRole;
        }

        return bestRoles.First();
    }

    private static Dictionary<GameRole, VoteStatistics> GetOtherPlayersWinRatesGivenMyRoleClaim(
        Player player, 
        GameState gameState, 
        GameState[] possibleEndStates)
    {
        Dictionary<GameRole, VoteStatistics> roleStats = new();

        // NOTE: No Distinct. We want this weighted for double inclusion where appropriate
        IEnumerable<GameRole> roles = gameState.Setup.Roles;
        
        foreach (var possibleRole in roles) {
            roleStats[possibleRole] = new();
            
            // Filter to roles we started as the role we're considering claiming
            GameState[] roleStates = possibleEndStates.Where(s => s.GetStartRole(player) == possibleRole).ToArray();

            foreach (var possibleState in roleStates)
            {
                roleStats[possibleRole].Support += possibleState.Support;
                // TODO: When we look at roles who want to be voted, this will need to be revisited
                roleStats[possibleRole].VotesReceived += possibleState.Events.OfType<VotedEvent>().Count(ve => ve.TargetPlayer == player);
            }
        }

        return roleStats;
    }
}