using MattEland.Wherewolf.Events.Game;

namespace MattEland.Wherewolf.Phases;

public class WakeUpPhase : GamePhase
{
    public override void Run(GameState newState, Action<GameState> callback)
    {
        newState.AddEvent(new MakeSocialClaimsNowEvent());
        callback(newState);
    }

    public override double Order => 900;
    public override string Name => "Day Phase Begins";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        yield return new GameState(priorState, priorState.Support);
    }
}