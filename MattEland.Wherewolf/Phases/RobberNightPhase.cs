using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class RobberNightPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        GameSlot? robber = newState.PlayerSlots.SingleOrDefault(p => p.StartRole.Name == "Robber");
        if (robber is not null)
        {
            // Figure out who we're robbing
            string[] targets = newState.Players.Where(p => p != robber.Player).Select(p => p.Name).ToArray();
            string targetName = robber.Player!.Controller.SelectRobberTarget(targets);
            GameSlot target = newState.PlayerSlots.Single(p => p.Name == targetName);
            
            PerformRobbery(newState, target, robber, broadcast: true);
        }

        return newState;
    }

    private static void PerformRobbery(GameState newState, GameSlot target, GameSlot robber, bool broadcast)
    {
        // Issue the event
        GameRole stolenRole = target.CurrentRole;
        newState.AddEvent(new RobbedPlayerEvent(robber.Player!, target.Player!, stolenRole), broadcastToController: broadcast);
            
        // Swap roles
        target.CurrentRole = robber.CurrentRole;
        robber.CurrentRole = stolenRole;
    }

    public override double Order => 6.0;
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        GameSlot? robber = priorState.PlayerSlots.SingleOrDefault(p => p.StartRole.Name == "Robber");

        if (robber is null)
        {
            // When no robber, no alterations occur, just skip the phase and move on
            yield return new GameState(priorState);
        }
        else
        {
            // When a robber is present, we spawn a new permutation per card they could have robbed
            foreach (var player in priorState.PlayerSlots.Where(p => p != robber))
            {
                GameState robbedState = new(priorState);
                
                PerformRobbery(robbedState, player, robber, broadcast: false);
                
                yield return robbedState;
            }
        }
    }
}