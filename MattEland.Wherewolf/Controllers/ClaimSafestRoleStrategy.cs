using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy : IRoleClaimStrategy
{
    private readonly Random _rand;

    public ClaimSafestRoleStrategy(Random rand)
    {
        _rand = rand;
    }
    
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(player);

        /*
        if (startRole.GetTeam() == Team.Villager)
        {
            return startRole;
        }
        */

        float best = -1;
        Dictionary<GameRole, float> roleVictoryProbabilities = new();

        foreach (var role in gameState.Roles.Distinct())
        {
            float probability = VotingHelper.GetRoleClaimWinProbabilityPerception(player, gameState, role);
            if (probability > best)
            {
                best = probability;
            }
            
            roleVictoryProbabilities[role] = probability;
        }
        
        return roleVictoryProbabilities
            .Where(kvp => Math.Abs(kvp.Value - best) < double.Epsilon)
            .MinBy(_ => _rand.Next())
            .Key;
    }
}