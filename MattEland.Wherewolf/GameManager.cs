using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf;

public class GameManager
{
    private readonly Queue<GamePhase> _remainingPhases;

    public GameManager(GameSetup setup)
    {
        CurrentState = setup.StartGame();
        _remainingPhases = new Queue<GamePhase>(setup.Phases);
        Players = setup.Players.ToArray();
        Roles = setup.Roles.ToArray();
    }
    
    public GameState CurrentState { get; private set; }
    public GamePhase? CurrentPhase => CurrentState.CurrentPhase;
    
    public bool IsGameOver => CurrentState.IsGameOver;
    public Player[] Players { get; }
    public GameRole[] Roles { get; }
    
    public IEnumerable<IGameEvent> Events => EventsForPlayer(null);

    public IEnumerable<IGameEvent> EventsForPlayer(Player? player = null)
    {
        foreach (var evt in CurrentState.Events)
        {
            // If it's a standard event and the player saw it or if we're omniscient, show it
            if (player is null || evt.IsObservedBy(player) || IsGameOver)
                yield return evt;

            // At this point we can marry in our social claims observed
            if (evt is MakeSocialClaimsNowEvent)
            {
                foreach (var social in CurrentState.Claims)
                {
                    yield return social;
                }
            }
        }
    }

    public void RunNext()
    {
        CurrentState.RunNext(s => CurrentState = s);
    }
    
    public void RunToEnd()
    {
        while (!IsGameOver)
        {
            RunNext();
        }
    }
    

    public void RunToEndOfNight()
    {
        if (CurrentPhase is not WakeUpPhase)
        {
            RunNext();
        }
    }    
    
    public void RunToVoting()
    {
        if (CurrentPhase is not VotingPhase)
        {
            RunNext();
        }
    }  
}