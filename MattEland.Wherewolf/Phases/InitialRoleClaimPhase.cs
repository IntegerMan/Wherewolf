using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class InitialRoleClaimPhase : GamePhase
{
    private readonly Player _player;

    public InitialRoleClaimPhase(Player player)
    {
        _player = player;
    }
    
    public override GameState Run(GameState newState)
    {
        GameRole role = _player.Controller.GetInitialRoleClaim(_player, newState);
        newState.AddEvent(new StartRoleClaimedEvent(_player, role));

        return newState;
    }

    public override double Order => 10000;
    public override string Name => $"Initial Role Claim ({_player.Name})";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // TODO: May need to create permutations potentially for each player's potential claims.
        yield return new GameState(priorState, priorState.Support);
    }
}