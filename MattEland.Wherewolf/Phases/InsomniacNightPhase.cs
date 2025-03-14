using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class InsomniacNightPhase : GamePhase
{
    public override void Run(PhaseContext context)
    {

        RunInsomniacPhase(context, broadcast: true);
        
        callback(newState);
    }

    public override double Order => 9.0;
    public override string Name => "Insomniac";

    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // The Insomniac role is non-interactive, so just run the night phase and record an event if it occurs
        GameState newState = new(priorState, priorState.Support);
        RunInsomniacPhase(newState, broadcast: false);

        yield return newState;
    }

    private static void RunInsomniacPhase(PhaseContext context, bool broadcast)
    {        
        context.AddEvent(EventPool.Announcement("Insomniac, wake up and look at your card.", GameRole.Insomniac), broadcast);
        foreach (var player in context.PlayersStartingInRole(GameRole.Insomniac))
        {
            context.AddEvent(EventPool.InsomniacSawCard(player.Name, context.GetCurrentRole(player)), broadcast);
        }
    }
}