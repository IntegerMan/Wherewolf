using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf.Phases;

public class InsomniacNightPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        InsomniacSawFinalCardEvent? insomniacEvent = BuildInsomniacEventIfRelevant(newState);
        if (insomniacEvent is not null)
        {
            newState.AddEvent(insomniacEvent);
        }

        return newState;
    }

    private static InsomniacSawFinalCardEvent? BuildInsomniacEventIfRelevant(GameState newState)
    {
        InsomniacSawFinalCardEvent? insomniacEvent;
        GameSlot? insomniac = newState.PlayerSlots.SingleOrDefault(p => p.StartRole.Name == "Insomniac");
        if (insomniac is not null)
        {
            insomniacEvent = new InsomniacSawFinalCardEvent(insomniac.Player!, insomniac.CurrentRole);
        }
        else
        {
            insomniacEvent = null;
        }

        return insomniacEvent;
    }

    public override double Order => 9.0;
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        // The Insomniac role is non-interactive, so just run the night phase and record an event if it occurs
        GameState newState = new(priorState, priorState.Phases.Skip(1));
        InsomniacSawFinalCardEvent? insomniacEvent = BuildInsomniacEventIfRelevant(newState);
        if (insomniacEvent is not null)
        {
            newState.AddEvent(insomniacEvent, broadcastToController: false);
        }

        yield return Run(newState);
    }
}