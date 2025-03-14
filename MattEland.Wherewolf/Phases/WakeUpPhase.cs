using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;

namespace MattEland.Wherewolf.Phases;

public class WakeUpPhase : GamePhase
{
    public override void Run(PhaseContext context)
    {
        context.AddEvent(EventPool.MakeSocialClaimsNow(), broadcast: true);
        callback(newState);
    }

    public override double Order => 900;
    public override string Name => "Day Phase Begins";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        yield return new GameState(priorState, priorState.Support);
    }
}