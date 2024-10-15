using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

/// <summary>
/// This class exists to help calculate the probabilities of players voting in a given way
/// based on a player's possible world states and possible role claims. The high-level approach
/// is to evaluate each possible game state and how a player would vote in each state based on
/// believing the claim, and how it would impact the player's win probability.
/// </summary>
public static class RoleClaimVotingProbabilities
{
    /// <summary>
    /// Calculates the claimer's likelihood percentage of being voted given a specific claim
    /// </summary>
    /// <param name="state">The game state, according to the claimer</param>
    /// <param name="claimer">The player potentially making the role claim</param>
    /// <param name="roleClaim">The role claim</param>
    /// <returns>The likelihood of being voted given that claim</returns>
    public static double CalculateLikelihoodOfBeingVoted(GameState state, Player claimer, GameRole roleClaim)
    {
        Random rand = new();
        long timesEvaluated = 0;
        long timesVoted = 0;
        
        PlayerProbabilities probabilities = state.CalculateProbabilities(claimer);
        
        foreach (var otherPlayer in state.Players.Where(p => p != claimer))
        {
            // Build a raw possible set of worlds that have the player as that role
            List<GameState> states = state.Setup.GetSetupPermutations();
            states = states.Where(s => s.GetStartRole(claimer) == roleClaim).ToList();
            
            // If the claimer cannot believe that the other player has the starting role for a world, remove it
            // For example, if a player knows they're the lone wolf, they won't consider that their target could be that role
            SlotRoleProbabilities recipStartProbs = probabilities.GetStartProbabilities(state.GetPlayerSlot(otherPlayer));
            foreach (var role in state.Setup.Roles)
            {
                if (recipStartProbs[role] <= 0)
                {
                    states = states.Where(s => s.GetStartRole(otherPlayer) != role).ToList();
                }
            }
            
            // TODO: We need to do something with the role claim here. Maybe add an event into a world copy or make an assumption?
            
            // Now that we have a full set of worlds, we need to figure out who the player votes for and tabulate the number of times it is the player
            foreach (var world in states)
            {
                Player vote = VotingHelper.GetMostLikelyWinningVote(world, otherPlayer, rand);
                timesEvaluated++;
                if (vote == claimer)
                {
                    timesVoted++;
                }
            }
        }

        if (timesEvaluated <= 0) return 0;
        
        // ReSharper disable once PossibleLossOfFraction
        return timesVoted / timesEvaluated;
    }
}