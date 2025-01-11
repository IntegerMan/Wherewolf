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
        GameRole role = _player.Controller.GetInitialRoleClaim(_player, newState.Parent!);
        newState.AddEvent(new StartRoleClaimedEvent(_player, role));

        return newState;
    }

    public override double Order => 10000;
    public override string Name => $"Initial Role Claim ({_player.Name})";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        List<GameRole> roles = priorState.Roles.Distinct().ToList();
        double support = priorState.Support / roles.Count;
        
        foreach (var role in roles)
        {
            GameState possibleState = new GameState(priorState, support);
            possibleState.AddEvent(new StartRoleClaimedEvent(_player, role), broadcastToController: false);
            
            yield return possibleState;
        }
    }
}