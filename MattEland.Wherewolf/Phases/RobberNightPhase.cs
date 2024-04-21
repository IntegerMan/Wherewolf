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
            
            // Issue the event
            GameRole stolenRole = target.CurrentRole;
            newState.AddEvent(new RobbedPlayerEvent(robber.Player!, target.Player!, stolenRole));
            
            // Swap roles
            target.CurrentRole = robber.CurrentRole;
            robber.CurrentRole = stolenRole;
        }

    return newState;
    }

    public override double Order => 2.0;
}