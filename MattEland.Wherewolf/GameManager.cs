using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf;

public class GameManager
{
    public GameManager(GameSetup setup)
    {
        CurrentState = setup.StartGame();
    }
    
    public GameState CurrentState { get; private set; }

    public void RunToEndOfNight()
    {
        CurrentState.RunToEndOfNight(state => CurrentState = state);
    }

    public void RunToEnd()
    {
        CurrentState.RunToEnd(state => CurrentState = state);
    }
}