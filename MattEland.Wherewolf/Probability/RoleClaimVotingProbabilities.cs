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
        
    }
}