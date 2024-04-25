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
    
    internal GameState(GameSetup setup, IReadOnlyList<GameRole> shuffledRoles)
    {
        setup.Validate();
        _gameSetup = setup;

        _playerSlots = BuildPlayerSlots(setup.Players, shuffledRoles);
        _centerSlots = BuildCenterSlots(setup.Players, shuffledRoles);

        AssignOrderIndexToEachPlayer(setup.Players);
        foreach (var slot in AllSlots)
        {
            AddEvent(new DealtCardEvent(slot.StartRole, slot), false);
        }
        _allPhases = setup.Phases;
        _remainingPhases = new Queue<GamePhase>(_allPhases);
        Root = this;
    }    

    internal GameState(GameState parentState)
    {
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases.Skip(1));
        _gameSetup = parentState._gameSetup;
        _allPhases = parentState._allPhases.ToArray();
        _events.AddRange(parentState.Events);
        _centerSlots = parentState.CenterSlots.Select(c => new GameSlot(c)).ToArray();
        _playerSlots = parentState.PlayerSlots.Select(c => new GameSlot(c)).ToArray();
        Parent = parentState;
        Root = parentState.Root;
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
        return shuffledRoles.Skip(players.Count())
                            .Select(r => new GameSlot("Center " + (c++), r))
                            .ToArray();
    }

    private static GameSlot[] BuildPlayerSlots(IEnumerable<Player> players, IReadOnlyList<GameRole> shuffledRoles)
    {
        int i = 0;
        return players.Select(p =>
        {
            GameRole role = shuffledRoles[i++];
            return new GameSlot(p.Name, role, p);
        }).ToArray();
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

    public PlayerProbabilities CalculateProbabilities(Player player)
    {
        PlayerProbabilities probabilities = new();

        // Start with all permutations
        List<GameEvent> observedEvents = Events.Where(e => e.IsObservedBy(player)).ToList();
        List<GamePermutation> validPermutations = _gameSetup.GetPermutationsAtPhase(this.CurrentPhase).Where(p => p.IsPossibleGivenEvents(observedEvents)).ToList();
        
        double startPopulation = validPermutations.Sum(p => p.Support);
        
        // Calculate starting role probabilities
        foreach (var slot in AllSlots)
        {
            foreach (var role in Roles.DistinctBy(r => r.Name))
            {
                // Figure out the number of possible worlds where the slot had the role at the start
                double startRoleSupport = validPermutations.Where(p => p.State.GetSlot(slot.Name).StartRole.Name == role.Name)
                                              .Sum(p => p.Support);

                probabilities.RegisterStartRoleProbabilities(slot, role.Name, startRoleSupport, startPopulation);
                
                // Figure out the number of possible worlds where the slot currently has the role
                double currentRoleSupport = validPermutations.Where(p => p.State.GetSlot(slot.Name).CurrentRole.Name == role.Name)
                    .Sum(p => p.Support);

                probabilities.RegisterCurrentRoleProbabilities(slot, role.Name, currentRoleSupport, startPopulation);
                
            }
        }
        
        return probabilities;
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

        GameState nextState = new(this);
        
        return CurrentPhase.Run(nextState);
    }

    public bool IsGameOver => _remainingPhases.Count == 0;
    public GamePhase? CurrentPhase => IsGameOver ? null : _remainingPhases.Peek();
    public IEnumerable<GamePhase> Phases => _remainingPhases.ToArray();
    public IEnumerable<GameState> PossibleNextStates 
    {
        get
        {
            if (IsGameOver)
            {
                yield break;
            }

            foreach (var possibleState in CurrentPhase!.BuildPossibleStates(this))
            {
                yield return possibleState;
            }
        }
    }
    public GameState? Parent { get; }
    public GameState Root { get; }

    public void AddEvent(GameEvent newEvent, bool broadcastToController = true)
    {
        _events.Add(newEvent);

        if (broadcastToController)
        {
            foreach (var player in Players)
            {
                if (newEvent.IsObservedBy(player))
                {
                    player.Controller.ObservedEvent(newEvent, this);
                }
            }
        }
    }

    public GameSlot GetPlayerSlot(Player player) 
        => _playerSlots.First(s => s.Player == player);

    public GameSlot GetSlot(string slotName)
        => AllSlots.First(s => s.Name == slotName);

    public override string ToString() 
        => $"{string.Join(",", PlayerSlots.Select(p => p.CurrentRole.Name))}[{string.Join(",", CenterSlots.Select(p => p.CurrentRole.Name))}]";

    internal void SwapRoles(GameSlot slot1, GameSlot slot2)
    {
        GameRole role1 = slot1.CurrentRole;
        GameRole role2 = slot2.CurrentRole;

        ReplaceSlot(slot1, new GameSlot(slot1, role2));
        ReplaceSlot(slot2, new GameSlot(slot2, role1));
    }

    private void ReplaceSlot(GameSlot oldSlot, GameSlot newSlot)
    {
        if (oldSlot.Player is not null)
        {
            int pIndex = _playerSlots.ToList().FindIndex(o => o.Name == oldSlot.Name);
            _playerSlots[pIndex] = newSlot;
        }
        else
        {
            int cIndex = _centerSlots.ToList().FindIndex(o => o.Name == oldSlot.Name);
            _centerSlots[cIndex] = newSlot;
        }
    }

    internal void SendRolesToControllers()
    {
        // In order to avoid sending events from possible worlds to players, we only broadcast dealt events after a root state has been chosen
        foreach (var dealtEvent in _events.OfType<DealtCardEvent>())
        {
            dealtEvent.Player?.Controller.ObservedEvent(dealtEvent, this);
        }
    }
}