using MattEland.Wherewolf.Events.Game;

namespace MattEland.Wherewolf.Phases;

/// <summary>
/// This is a game phase that exists for any special initial setup or diagnostics work. It doesn't normally fit into a game flow
/// </summary>
public class SetupNightPhase : GamePhase
{
    public override string Name => "Setup";
    
    public override void Run(GameState newState, Action<GameState> callback)
    {
        newState.SendRolesToControllers();
        newState.AddEvent(new GamePhaseAnnouncedEvent("Everyone, go to sleep.", null));

        callback(newState);
    }

    public override double Order => double.MinValue;
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        yield return new GameState(priorState, priorState.Support);
    }
}