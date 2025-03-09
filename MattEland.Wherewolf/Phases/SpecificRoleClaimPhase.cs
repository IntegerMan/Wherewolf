using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class SpecificRoleClaimPhase(Player player) : GamePhase
{
    public override void Run(GameState newState, Action<GameState> callback)
    {
        // At this point they've already made a claim, so let's find it.
        GameRole initialClaim = newState.Claims.OfType<StartRoleClaimedEvent>()
            .Single(c => c.Player == player)
            .ClaimedRole;
        
        SpecificRoleClaim[] possibleClaims = newState.GeneratePossibleSpecificRoleClaims(player);
        
        player.Controller.GetSpecificRoleClaim(player, newState.Parent!, initialClaim, possibleClaims, claim =>
        {
            newState.AddEvent(claim);
            callback(newState);
        });
    }

    public override double Order => 20000;
    public override string Name => $"Specific Role Claim ({player.Name})";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // No branching needed for this. Branching to all possible claims is not necessary and combinatorially expensive
        yield return new GameState(priorState, priorState.Support);
    }
}