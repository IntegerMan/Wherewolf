using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy(Random rand) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(player);
        /*
        // Early hack: If we're a villager, claim villager
        if (startRole.GetTeam() == Team.Villager)
        {
            return startRole;
        }
        */
        
        // We need to look at how we want other people to vote, based on the worlds we believe could be possible.
        // Based on these worlds, we want a win % based on who every other player votes for.
        //IDictionary<string, VoteVictoryStatistics> stats = VotingHelper.BuildOtherPlayerVoteVictoryStatistics(player, gameState);

        // Now, let's take roles into account and see what victory % each other player gets voting for us based on who they think we are
        Dictionary<GameRole, Dictionary<Player, VoteWinProbabilityStatistics>> roleStats = GetOtherPlayerVotingForPlayerRoleClaimVictoryStats(player, gameState);

        // We then tabulate the % of times we get voted for based on the role we're considering claiming
        Dictionary<GameRole, float> roleVotedPercent = new();
        foreach (var kvp in roleStats)
        {
            float totalSupport = kvp.Value.Values.Sum(v => v.Support);
            float totalWinProbability = kvp.Value.Values.Sum(v => v.TotalWinProbability);
            roleVotedPercent[kvp.Key] = totalWinProbability / totalSupport;
        }
        
        // Now let's consider the top role based on the highest win % for us
        float best = roleVotedPercent.Values.Max();
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

    private static Dictionary<GameRole, Dictionary<Player, VoteWinProbabilityStatistics>> GetOtherPlayerVotingForPlayerRoleClaimVictoryStats(Player player, GameState gameState)
    {
        Player[] otherPlayers = gameState.Players.Where(p => p != player).ToArray();
        GameState[] possibleStates = gameState.Setup.GetPermutationsAtPhase(gameState.CurrentPhase).ToArray();
        Dictionary<GameRole, Dictionary<Player, VoteWinProbabilityStatistics>> roleStats = new();
        foreach (var possibleRole in gameState.Setup.Roles) // NOTE: No Distinct. We want this weighted for double inclusion where appropriate
        {
            // Filter to roles we started as the role we're considering claiming
            GameState[] roleStates = possibleStates.Where(s => s.GetStartRole(player) == possibleRole).ToArray();
            
            foreach (var otherPlayer in otherPlayers)
            {
                foreach (var possibleState in roleStates)
                {
                    Dictionary<Player, float> observedVictoryPercent = VotingHelper.GetVoteVictoryProbabilities(otherPlayer, possibleState);
                    
                    if (!roleStats.ContainsKey(possibleRole))
                    {
                        roleStats[possibleRole] = new();
                    }

                    if (roleStats[possibleRole].TryGetValue(otherPlayer, out VoteWinProbabilityStatistics? value))
                    {
                        value.Support++;
                        value.TotalWinProbability += observedVictoryPercent[player];
                    }
                    else
                    {
                        roleStats[possibleRole][otherPlayer] = new()
                        {
                            Support = 1,
                            TotalWinProbability = observedVictoryPercent[player]
                        };
                    }
                }
            }
        }

        return roleStats;
    }
}