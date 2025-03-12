using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class InsomniacNightPhase : GamePhase
{
    public override void Run(GameState newState, Action<GameState> callback)
    {
        newState.AddEvent(EventPool.Announcement("Insomniac, wake up and look at your card.", GameRole.Insomniac));

        RunInsomniacPhase(newState, broadcast: true);
        
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

    private static void RunInsomniacPhase(GameState state, bool broadcast)
    {
        foreach (var insomniac in state.PlayerSlots.Where(p => state.GetStartRole(p) == GameRole.Insomniac))
        {
            state.AddEvent(EventPool.InsomniacSawCard(insomniac.Player!, insomniac.Role), broadcast);
        }
    }
}