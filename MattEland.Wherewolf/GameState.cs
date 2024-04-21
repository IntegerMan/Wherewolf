using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameState
{
    private readonly GameSetup _gameSetup;
    private readonly GameSlot[] _centerSlots;
    private readonly GameSlot[] _playerSlots;
    private readonly GamePhase[] _allPhases;
    private readonly Queue<GamePhase> _remainingPhases;
    private readonly List<GameEvent> _events = new();

    internal GameState(GameSetup setup, ISlotShuffler shuffler) 
        :this(setup, shuffler.Shuffle(setup.Roles).ToList())
    {
    }
    
    internal GameState(GameSetup setup, IReadOnlyList<GameRole> shuffledRoles)
    {
        setup.Validate();
        _gameSetup = setup;

        _playerSlots = BuildPlayerSlots(setup.Players, shuffledRoles);
        _centerSlots = BuildCenterSlots(setup.Players, shuffledRoles);

        AssignOrderIndexToEachPlayer(setup.Players);
        RegisterStartingCards();
        _allPhases = BuildGamePhases(setup.Roles);
        _remainingPhases = new Queue<GamePhase>(_allPhases);
    }    

    private GameState(GameState oldState, IEnumerable<GamePhase> remainingPhases)
    {
        _remainingPhases = new Queue<GamePhase>(remainingPhases);
        _gameSetup = oldState._gameSetup;
        _allPhases = oldState._allPhases.ToArray();
        _events.AddRange(oldState.Events);
        _centerSlots = oldState.CenterSlots.ToArray();
        _playerSlots = oldState.PlayerSlots.ToArray();
    }

    private GamePhase[] BuildGamePhases(IEnumerable<GameRole> roles)
    {
        List<GamePhase> phases = new();
        foreach (var role in roles.Where(r => r.HasNightPhases).DistinctBy(r => r.GetType()))
        {
            phases.AddRange(role.BuildNightPhases());
        }

        return phases.OrderBy(p => p.Order).ToArray();
    }

    private static void AssignOrderIndexToEachPlayer(IEnumerable<Player> players)
    {
        int order = 0;
        foreach (var player in players)
        {
            player.Order = order++;
        }
    }

    private static GameSlot[] BuildCenterSlots(IEnumerable<Player> players, IEnumerable<GameRole> shuffledRoles)
    {
        int c = 1;
        return shuffledRoles.Skip(players.Count()).Select(r => new GameSlot("Center " + (c++), r)).ToArray();
    }

    private static GameSlot[] BuildPlayerSlots(IEnumerable<Player> players, IReadOnlyList<GameRole> shuffledRoles)
    {
        int i = 0;
        return players.Select(p => new GameSlot(p.Name, shuffledRoles[i++]) { Player = p}).ToArray();
    }

    private void RegisterStartingCards()
    {
        foreach (var slot in AllSlots)
        {
            AddEvent(new DealtCardEvent(slot.StartRole, slot));
        }
    }

    public GameSlot[] PlayerSlots => _playerSlots;
    public GameSlot[] CenterSlots => _centerSlots;

    public IEnumerable<GameSlot> AllSlots
    {
        get
        {
            foreach (var slot in _playerSlots)
            {
                yield return slot;
            }

            foreach (var slot in _centerSlots)
            {
                yield return slot;
            }
        }
    }

    public IEnumerable<Player> Players => _gameSetup.Players;
    public IEnumerable<GameRole> Roles => _gameSetup.Roles;
    public IEnumerable<GameEvent> Events => _events.AsReadOnly();

    public PlayerState GetPlayerStates(Player player)
    {
        PlayerState state = new(player, _gameSetup);
        
        state.AddEvents(_events.Where(e => e.IsObservedBy(player)));

        return state;
    }

    public GameState RunToEnd()
    {
        if (IsGameOver)
        {
            return this;
        }

        GameState nextState = RunNext();
        return nextState.RunToEnd();
    }

    private GameState RunNext()
    {
        if (CurrentPhase is null)
            throw new InvalidOperationException("Cannot run the next phase; no current phase");

        GameState nextState = new(this, _remainingPhases.Skip(1));
        
        return CurrentPhase.Run(nextState);
    }

    public bool IsGameOver => _remainingPhases.Count == 0;
    public GamePhase? CurrentPhase => IsGameOver ? null : _remainingPhases.Peek();
    public IEnumerable<GamePhase> Phases => _remainingPhases.ToArray();

    public void AddEvent(GameEvent newEvent)
    {
        _events.Add(newEvent);
    }

    public GameSlot GetPlayerSlot(Player player) 
        => _playerSlots.First(s => s.Player == player);

    public GameSlot GetSlot(string slotName)
        => AllSlots.First(s => s.Name == slotName);
}