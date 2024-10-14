using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

/// <summary>
/// This class exists to help calculate the probabilities of players voting in a given way
/// based on a player's possible world states and possible role claims. The high-level approach
/// is to evaluate each possible game state and how a player would vote in each state based on
/// believing the claim, and how it would impact the player's win probability.
/// </summary>
public class RoleClaimVotingProbabilities
{
    public void CalculateVoteImpact(GameState state, Player claimer, GameRole roleClaim)
    {
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
            
            // Now that we have a full set of worlds, we need to figure out who the player votes for
            Dictionary<Player, int> votes = new();
            foreach (var world in states)
            {
                // TODO: This is probably not good enough, and definitely won't work with human players
                Player vote = otherPlayer.Controller.GetPlayerVote(otherPlayer, world);
                if (!votes.TryAdd(vote, 1))
                {
                    votes[vote]++;
                }
            }
        }
    }
}