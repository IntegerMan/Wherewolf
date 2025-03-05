using MattEland.Wherewolf.Events.Game;

namespace MattEland.Wherewolf.Phases;

public class WakeUpPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        newState.AddEvent(new MakeSocialClaimsNowEvent());
        return newState;
    }

    public override double Order => 900;
    public override string Name => "Day Phase Begins";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        yield return new GameState(priorState, priorState.Support);
    }
}