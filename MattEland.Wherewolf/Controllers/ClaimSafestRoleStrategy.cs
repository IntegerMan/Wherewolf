using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class ClaimSafestRoleStrategy(Random rand) : IRoleClaimStrategy
{
    public GameRole GetRoleClaim(Player player, GameState gameState)
    {
        GameRole startRole = gameState.GetStartRole(player);
        Team startTeam = startRole.GetTeam();
        Team currentTeam = gameState.GetSlot(player).Role.GetTeam();

        // Pre-compute whether claiming the true start role enables a truthful, other-confirmable
        // specific claim. Only relevant when the player is currently village-aligned.
        bool hasConfirmableClaim = currentTeam != Team.Werewolf
            && gameState.GeneratePossibleSpecificRoleClaims(player)
                .Any(c => c.Role == startRole
                    && c.IsClaimValidFor(gameState)
                    && c.IsConfirmableByOtherPlayer(gameState));

        // When there is a confirmable specific claim, prefer the true start role unconditionally.
        // The VoteFactor statistics do not yet model the belief benefit of a verifiable claim,
        // so we short-circuit rather than let an inflated vote count push us toward a safe lie.
        if (hasConfirmableClaim)
        {
            return startRole;
        }

        KeyValuePair<GameRole, VoteStatistics>[] roleClaimVoteStats =
            VotingHelper.GetRoleClaimVoteStatistics(player, gameState).ToArray();

        double best = roleClaimVoteStats.Min(rs => rs.Value.VoteFactor);

        // Consider the top role based on the lowest win % for our opponents voting for who they think we are
        // This will encourage us to claim the role that is least likely to get us killed
        GameRole[] bestRoles = roleClaimVoteStats
            .Where(rs => rs.Value.VoteFactor <= best)
            // This makes the werewolf team more likely to claim safe roles while villager team more likely to claim their own role
            .OrderByDescending(rs => startTeam == Team.Werewolf ? rs.Value.OutOfPlayPercent : rs.Value.InPlayPercent)
            // High support are more likely to be true
            .ThenByDescending(rs => rs.Value.Support)
            // Low claims are good
            .ThenBy(rs => rs.Value.OtherClaims)
            // Prefer start role when claiming it unlocks a verifiable truthful specific claim
            .ThenByDescending(rs => hasConfirmableClaim && rs.Key == startRole)
            // Resolve ties with randomness
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

    public SpecificRoleClaim GetSpecificRoleClaim(Player player, GameState gameState, SpecificRoleClaim[] possibleClaims,
        GameRole initialClaim)
    {
        Team currentTeam = gameState.GetSlot(player).Role.GetTeam();

        // Short-circuit: when a truthful, other-confirmable specific claim exists and we are
        // village-aligned, prefer it unconditionally. The VoteFactor statistics do not model
        // the belief benefit of a verified claim, so we bypass the statistical analysis.
        if (currentTeam != Team.Werewolf)
        {
            SpecificRoleClaim? confirmable = possibleClaims
                .FirstOrDefault(c => c.IsClaimValidFor(gameState) && c.IsConfirmableByOtherPlayer(gameState));
            if (confirmable is not null)
            {
                return confirmable;
            }
        }

        KeyValuePair<SpecificRoleClaim, VoteStatistics>[] options =
            VotingHelper.GetSpecificRoleClaimVoteStatistics(player, gameState, possibleClaims)
                .Where(o => o.Value.Support > 0)
                .OrderBy(o => o.Value.VoteFactor)
                .ToArray();

        double best = options.Min(rs => rs.Value.VoteFactor);
        SpecificRoleClaim[] rankedChoices = options
            .Where(rs => rs.Value.VoteFactor <= best)
            // Top priority: prefer a truthful claim that another player can independently verify (village team only)
            .OrderByDescending(rs => currentTeam != Team.Werewolf
                && rs.Key.IsClaimValidFor(gameState)
                && rs.Key.IsConfirmableByOtherPlayer(gameState))
            // Stick with our initial role claim if possible
            .ThenByDescending(rs => rs.Key.Role == initialClaim)
            // This makes the werewolf team more likely to claim safe roles while villager team more likely to claim their own role
            .ThenByDescending(rs => rs.Value.InPlayPercent)
            // High support are more likely to be true
            .ThenByDescending(rs => rs.Value.Support)
            // Low claims are good
            .ThenBy(rs => rs.Value.OtherClaims)
            // Resolve ties with randomness
            .ThenBy(_ => rand.Next())
            .Select(o => o.Key)
            .ToArray();

        return rankedChoices.FirstOrDefault() ?? options.First().Key;
    }
}