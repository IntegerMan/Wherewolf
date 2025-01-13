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
    /// <param name="otherPlayer">The player we're evaluating the role claim for</param>
    /// <param name="roleClaim">The role claim</param>
    /// <returns>The likelihood of being voted given that claim</returns>
    public static double CalculateAverageBeliefProbability(GameState state, Player claimer, Player otherPlayer, GameRole roleClaim)
    {
        int timesEvaluated = 0;
        double totalProbability = 0;
        
        IEnumerable<GameState> permutations = VotingHelper.GetPossibleGameStatesForPlayer(claimer, state);

        foreach (var perm in permutations)
        {
            PlayerProbabilities targetProbs = perm.CalculateProbabilities(otherPlayer);
            SlotRoleProbabilities claimerStartProbs = targetProbs.GetStartProbabilities(perm.GetSlot(claimer));
            SlotProbability slotProbs = claimerStartProbs[roleClaim];
            totalProbability += slotProbs.Probability;
            timesEvaluated++;
        }
        
        return totalProbability / timesEvaluated;
    }    
    
    /// <summary>
    /// Calculates the claimer's likelihood percentage of being voted given a specific claim as an average of other players
    /// </summary>
    /// <param name="state">The game state, according to the claimer</param>
    /// <param name="claimer">The player potentially making the role claim</param>
    /// <param name="roleClaim">The role claim</param>
    /// <returns>The likelihood of being voted given that claim</returns>
    public static double CalculateAverageBeliefProbability(GameState state, Player claimer, GameRole roleClaim)
    {
        int otherPlayers = state.Players.Count() - 1;
        
        double totalProbability = 0;
        foreach (var player in state.Players.Where(p => p != claimer))
        {
            totalProbability += CalculateAverageBeliefProbability(state, claimer, player, roleClaim);
        }
        
        return totalProbability / otherPlayers;
    }
}