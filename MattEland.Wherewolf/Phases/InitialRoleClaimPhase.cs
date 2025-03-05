using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class InitialRoleClaimPhase(Player player) : GamePhase
{
    public override GameState Run(GameState newState)
    {
        GameRole role = player.Controller.GetInitialRoleClaim(player, newState.Parent!);
        newState.AddEvent(new StartRoleClaimedEvent(player, role));

        return newState;
    }

    public override double Order => 10000;
    public override string Name => $"Initial Role Claim ({player.Name})";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // No branching needed for this. Branching to all possible claims is not necessary and combinatorially expensive
        yield return new GameState(priorState, priorState.Support);
    }
}