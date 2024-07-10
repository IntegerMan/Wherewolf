using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class RobberNightPhase : GamePhase
{
    public override string Name => "Robber";
    
    public override GameState Run(GameState newState)
    {
        GameSlot? robber = newState.PlayerSlots.SingleOrDefault(p => newState.GetStartRole(p) == GameRole.Robber);
        if (robber is not null)
        {
            // Figure out who we're robbing
            string[] targets = newState.Players.Where(p => p != robber.Player)
                                               .Select(p => p.Name)
                                               .ToArray();
            string targetName = robber.Player!.Controller.SelectRobberTarget(targets);
            GameSlot target = newState.PlayerSlots.Single(p => p.Name == targetName);
            
            newState = PerformRobbery(newState, target, robber, broadcast: true);
        }

        return newState;
    }

    private static GameState PerformRobbery(GameState newState, GameSlot target, GameSlot robber, bool broadcast)
    {
        // Issue the event
        GameRole stolenRole = target.Role;
        RobbedPlayerEvent robbedEvent = new(robber.Player!, target.Player!, stolenRole);
        newState.AddEvent(robbedEvent, broadcastToController: broadcast);
            
        // Swap roles
        return newState.SwapRoles(target.Name, robber.Name);
    }

    public override double Order => 6.0;
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        Player? robberPlayer = priorState.PlayerSlots.Where(p => priorState.GetStartRole(p) == GameRole.Robber).Select(p => p.Player).FirstOrDefault();

        if (robberPlayer is null)
        {
            // When no robber, no alterations occur, just skip the phase and move on
            yield return new GameState(priorState, priorState.Support);
        }
        else
        {
            // When a robber is present, we spawn a new permutation per card they could have robbed
            Player[] eligibleTargets = priorState.Players.Where(p => p != robberPlayer).ToArray();
            foreach (var targetPlayer in eligibleTargets)
            {
                GameState robbedState = new(priorState, priorState.Support / eligibleTargets.Length);

                GameSlot robber = robbedState[robberPlayer.Name];
                GameSlot target = robbedState[targetPlayer.Name];

                GameState stateAfterRobbery = PerformRobbery(robbedState, target, robber, broadcast: false);
                
                yield return stateAfterRobbery;
            }
        }
    }
}