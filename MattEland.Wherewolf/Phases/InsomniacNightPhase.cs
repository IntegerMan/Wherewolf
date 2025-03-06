using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class InsomniacNightPhase : GamePhase
{
    public override void Run(GameState newState, Action<GameState> callback)
    {
        newState.AddEvent(new GamePhaseAnnouncedEvent("Insomniac, wake up and look at your card."));
        
        InsomniacSawFinalCardEvent? insomniacEvent = BuildInsomniacEventIfRelevant(newState);
        if (insomniacEvent is not null)
        {
            newState.AddEvent(insomniacEvent);
        }
        
        callback(newState);
    }

    private static InsomniacSawFinalCardEvent? BuildInsomniacEventIfRelevant(GameState newState)
    {
        GameSlot? insomniac = newState.PlayerSlots.SingleOrDefault(p => newState.GetStartRole(p) == GameRole.Insomniac);
        return insomniac is not null 
            ? new InsomniacSawFinalCardEvent(insomniac.Player!, insomniac.Role) 
            : null;
    }

    public override double Order => 9.0;
    public override string Name => "Insomniac";

    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // The Insomniac role is non-interactive, so just run the night phase and record an event if it occurs
        GameState newState = new(priorState, priorState.Support);
        InsomniacSawFinalCardEvent? insomniacEvent = BuildInsomniacEventIfRelevant(newState);
        if (insomniacEvent is not null)
        {
            newState.AddEvent(insomniacEvent, broadcastToController: false);
        }

        yield return newState;
    }
}