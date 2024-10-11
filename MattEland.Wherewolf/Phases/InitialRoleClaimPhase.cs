using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class InitialRoleClaimPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        foreach (var player in newState.Players)
        {
            GameRole role = player.Controller.GetInitialRoleClaim(newState);
            newState.AddEvent(new RoleClaimedEvent(player, role));
        }

        return newState;
    }

    public override double Order => 10000;
    public override string Name => "Initial Role Claim";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // TODO: May need to create permutations potentially for each player's potential claims.
        yield return new GameState(priorState, priorState.Support);
    }
}