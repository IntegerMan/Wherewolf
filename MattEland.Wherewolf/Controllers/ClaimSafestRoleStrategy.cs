using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy(Random rand) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(player);
        GameState[] possibleEndStates = gameState.Setup.GetPermutationsAtPhase(null).ToArray();
        Player[] otherPlayers = gameState.Players.Where(p => p != player).ToArray();

        // Now, let's take roles into account and see what victory % each other player gets voting for us based on who they think we are
        Dictionary<GameRole, Dictionary<Player, VoteVictoryStatistics>> roleStats 
            = GetOtherPlayerWinStatsGivenMyRoleClaim(player, gameState, possibleEndStates, otherPlayers);

        // We then tabulate the % of times we get voted for based on the role we're considering claiming
        Dictionary<GameRole, double> roleVotedPercent = new();
        foreach (var kvp in roleStats)
        {
            double totalSupport = kvp.Value.Values.Sum(v => v.Support);
            double totalWinProbability = kvp.Value.Values.Sum(v => v.WinPercent);
            roleVotedPercent[kvp.Key] = totalWinProbability / totalSupport;
        }
        
        // Now let's consider the top role based on the highest win % for us
        double best = roleVotedPercent.Values.Max();
        GameRole[] bestRoles = roleVotedPercent
            .Where(kvp => kvp.Value >= best)
            .Select(kvp => kvp.Key)
            .ToArray();
        
        // If there's only one role, we're done
        if (bestRoles.Length == 1)
        {
            return bestRoles[0];
        }
        
        // If we have a tie, we want to go with the role we actually started as if it's in the tie
        if (bestRoles.Contains(startRole))
        {
            return startRole;
        }
        
        // If we don't have a tie, we want to go with a random role in the list
        return bestRoles[rand.Next(bestRoles.Length)];
    }

    private static Dictionary<GameRole, Dictionary<Player, VoteVictoryStatistics>> GetOtherPlayerWinStatsGivenMyRoleClaim(Player player, GameState gameState, GameState[] possibleEndStates, Player[] otherPlayers)
    {
        Dictionary<GameRole, Dictionary<Player, VoteVictoryStatistics>> roleStats = new();
        
        foreach (var possibleRole in gameState.Setup.Roles) // NOTE: No Distinct. We want this weighted for double inclusion where appropriate
        {
            roleStats[possibleRole] = new();
            
            // Filter to roles we started as the role we're considering claiming
            GameState[] roleStates = possibleEndStates.Where(s => s.GetStartRole(player) == possibleRole).ToArray();

            foreach (var possibleState in roleStates)
            {
                foreach (var otherPlayer in otherPlayers)
                {
                    bool isWin = possibleState.GameResult!.DidPlayerWin(otherPlayer);

                    if (roleStats[possibleRole].TryGetValue(otherPlayer, out VoteVictoryStatistics? value))
                    {
                        value.Support++;
                        value.Wins += isWin ? 1 : 0;
                    }
                    else
                    {
                        roleStats[possibleRole][otherPlayer] = new()
                        {
                            Support = 1,
                            Wins = isWin ? 1 : 0
                        };
                    }
                }
            }
        }

        return roleStats;
    }
}