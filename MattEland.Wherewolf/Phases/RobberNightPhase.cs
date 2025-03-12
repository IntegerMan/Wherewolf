using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class RobberNightPhase : GamePhase
{
    public override string Name => "Robber";

    private void AddRobberNightAnnouncement(GameState newState, bool broadcast)
    {
        newState.AddEvent(EventPool.Announcement("Robber, wake up and exchange cards with another player, then look at your new card.", GameRole.Robber), broadcast);
    }

    public override void Run(GameState newState, Action<GameState> callback)
    {
        GameSlot? robber = newState.PlayerSlots.SingleOrDefault(p => newState.GetStartRole(p) == GameRole.Robber);
        if (robber is not null)
        {
            // Figure out who we're robbing
            Player[] targets = newState.Players.Where(p => p != robber.Player)
                                               .ToArray();
            robber.Player!.Controller.SelectRobberTarget(targets, newState, robber.Player, target =>
            {
                newState = PerformRobbery(newState, newState.GetSlot(target), robber, broadcast: true);
                callback(newState);
            });
        }
        else
        {
            AddRobberNightAnnouncement(newState, broadcast: true);
            callback(newState);
        }
    }

    private GameState PerformRobbery(GameState newState, GameSlot target, GameSlot robber, bool broadcast)
    {
        GameRole stolenRole = target.Role;
            
        // Swap roles
        GameState swappedState = newState.SwapRoles(target.Name, robber.Name);
        
        AddRobberNightAnnouncement(swappedState, broadcast);
        swappedState.AddEvent(EventPool.Robbed(robber.Player!, target.Player!, stolenRole), broadcast);
        
        return swappedState;
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