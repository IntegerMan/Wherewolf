using MattEland.Wherewolf.Events.Social;

namespace MattEland.Wherewolf.Phases;

public class InitialRoleClaimPhase(Player player) : GamePhase
{
    public override void Run(GameState newState, Action<GameState> callback)
    {
        player.Controller.GetInitialRoleClaim(player, newState.Parent!, role =>
        {
            newState.AddEvent(new StartRoleClaimedEvent(player, role, newState));
            callback(newState);
        });
    }

    public override double Order => 10000;
    public override string Name => $"Initial Role Claim ({player.Name})";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // No branching needed for this. Branching to all possible claims is not necessary and combinatorially expensive
        yield return new GameState(priorState, priorState.Support);
    }
}