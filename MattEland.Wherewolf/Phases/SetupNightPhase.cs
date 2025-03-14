using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;

namespace MattEland.Wherewolf.Phases;

/// <summary>
/// This is a game phase that exists for any special initial setup or diagnostics work. It doesn't normally fit into a game flow
/// </summary>
public class SetupNightPhase : GamePhase
{
    public override string Name => "Setup";
    
    public override void Run(PhaseContext context)
    {
        newState.SendRolesToControllers();
        context.AddEvent(EventPool.Announcement("Everyone, go to sleep."), broadcast: true);

        callback(newState);
    }

    public override double Order => double.MinValue;
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        yield return new GameState(priorState, priorState.Support);
    }
}