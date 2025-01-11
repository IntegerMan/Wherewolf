using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MoreLinq.Extensions;

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
        
        // Early hack: If we're a villager, claim villager
        if (role.GetTeam() == Team.Villager)
        {
            return role;
        }
        
        // We need to look at how we want other people to vote, based on the worlds we believe could be possible.
        // Based on these worlds, we want a win % based on who every other player votes for.
        IDictionary<string, VoteStatistics> stats = VotingHelper.BuildOtherPlayerVoteVictoryStatistics(player, gameState);

        // Now we want to see that player's win % based on their role and who they believe we are
        
        // We then tabluate our win % based on who we believe they are and how we expect them to vote given those combinations
        
        // We then go with the highest win % based on these combinations, using our actual role in case of a tie, or the first match if none of the tied are our actual role
        
        return role;
    }
}