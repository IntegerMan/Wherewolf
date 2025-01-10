using MattEland.Wherewolf.Events;
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
        float best = -1f;
        GameRole role = gameState.GetStartRole(player);
        foreach (var possibleNextState in gameState.PossibleNextStates.OrderBy(_ => _rand.Next()))
        {
            float winPercent = VotingHelper.GetStateVictoryPercent(player, possibleNextState);
            if (winPercent > best)
            {
                best = winPercent;
                StartRoleClaimedEvent claimEvent = possibleNextState.Claims.First(e => e.Player == player);

                role = claimEvent.ClaimedRole;
            }
        }
        
        return role;
    }
}