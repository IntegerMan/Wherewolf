using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf;

public class GameState
{
    private readonly GameSetup _gameSetup;
    private readonly GameSlot[] _centerSlots;
    private readonly GameSlot[] _playerSlots;
    private readonly Queue<GamePhase> _remainingPhases;
    private readonly List<GameEvent> _events = new();
    private readonly Dictionary<string, GameSlot> _slots = new();
    private readonly List<StartRoleClaimedEvent> _claims;
    public double Support { get; }
    
    internal GameState(GameSetup setup, IReadOnlyList<GameRole> shuffledRoles, double support)
    {
        setup.Validate();
        _gameSetup = setup;

        _playerSlots = BuildPlayerSlots(setup.Players, shuffledRoles);
        _centerSlots = BuildCenterSlots(setup.Players, shuffledRoles);
        foreach(var slot in AllSlots)
        {
            _slots[slot.Name] = slot;
        }

        AssignOrderIndexToEachPlayer(setup.Players);
        foreach (var slot in AllSlots)
        {
            AddEvent(new DealtCardEvent(slot.Role, slot), false);
        }

        _claims = new List<StartRoleClaimedEvent>(_playerSlots.Length);
        _remainingPhases = new Queue<GamePhase>(setup.Phases);
        Root = this;
        Support = support;
    }    

    internal GameState(GameState parentState, double support)
    {
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases.Skip(1));
        _gameSetup = parentState._gameSetup;
        _events.AddRange(parentState.Events);
        _claims = _events.OfType<StartRoleClaimedEvent>().ToList();
        _centerSlots = parentState.CenterSlots.Select(c => new GameSlot(c)).ToArray();
        _playerSlots = parentState.PlayerSlots.Select(c => new GameSlot(c)).ToArray();
        foreach(var slot in AllSlots)
        {
            _slots[slot.Name] = slot;
        }
        Parent = parentState;
        Root = parentState.Root;
        Support = support;
    }
    
    /// <summary>
    /// Create a gamestate variant with a fixed set of slots. This method is used for swapping cards only.
    /// </summary>
    internal GameState(GameState parentState, IEnumerable<GameSlot> playerSlots, IEnumerable<GameSlot> centerSlots)
    {
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases);
        _gameSetup = parentState._gameSetup;
        _events.AddRange(parentState.Events);
        _claims = _events.OfType<StartRoleClaimedEvent>().ToList();
        _centerSlots = centerSlots.ToArray();
        _playerSlots = playerSlots.ToArray();
        
        foreach(var slot in AllSlots)
        {
            _slots[slot.Name] = slot;
        }
        Parent = parentState.Parent;
        Root = parentState.Root;
        Support = parentState.Support;
    }
    
    /// <summary>
    /// This constructor is intended only for unit tests
    /// </summary>
    public GameState(IEnumerable<Player> players, IEnumerable<GameRole> roles, IEnumerable<GamePhase> remainingPhases, IEnumerable<GameEvent> priorEvents)
    {
        GameSetup setup = new();
        setup.AddPlayers(players.ToArray());
        setup.AddRoles(roles.ToArray());
        
        _gameSetup = setup;
        _remainingPhases = new Queue<GamePhase>(remainingPhases);
        _events.AddRange(priorEvents);
        _claims = _events.OfType<StartRoleClaimedEvent>().ToList();
        _playerSlots = BuildPlayerSlots(setup.Players, setup.Roles.ToList());
        _centerSlots = BuildCenterSlots(setup.Players, setup.Roles.ToList());
        foreach(var slot in AllSlots)
        {
            _slots[slot.Name] = slot;
        }
        
        Parent = null;
        Root = this;
        Support = 1;
    }

    public IEnumerable<StartRoleClaimedEvent> Claims => _claims;
    
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
        IEnumerable<GameState> validPermutations = VotingHelper.GetPossibleGameStatesForPlayer(player, this);

        double startPopulation = validPermutations.Sum(p => p.Support);
        
        IEnumerable<StartRoleClaimedEvent> claimedEvents = Events.OfType<StartRoleClaimedEvent>();

        // Calculate starting role probabilities
        foreach (var slot in AllSlots)
        {
            foreach (var role in Roles.Distinct())
            {
                // Figure out the number of possible worlds where the slot had the role at the start
                double startRoleSupport = validPermutations.Where(p => p.Root[slot.Name].Role == role)
                                              .Sum(p => p.Support);

                IEnumerable<Player> startSupport = claimedEvents
                    .Where(e => e.ClaimedRole == role && e.Player == slot.Player && e.IsClaimValidFor(this))
                    .Select(e => e.Player)
                    .Where(e => e != player)
                    .Distinct();
                
                probabilities.RegisterStartRoleProbabilities(slot, role, startRoleSupport, startPopulation, startSupport);
                
                // Figure out the number of possible worlds where the slot currently has the role
                IEnumerable<GameState> endGameStates = validPermutations.Where(p => p[slot.Name].Role == role);
                double currentRoleSupport = endGameStates.Sum(p => p.Support);
                
                probabilities.RegisterCurrentRoleProbabilities(slot, role, currentRoleSupport, startPopulation, Enumerable.Empty<Player>());
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
    

    public GameState RunToEndOfNight()
    {
        if (CurrentPhase is WakeUpPhase)
        {
            return this;
        }

        GameState nextState = RunNext();
        return nextState.RunToEndOfNight();
    }    

    private GameState RunNext()
    {
        GamePhase? phase = CurrentPhase;
        if (phase is null)
            throw new InvalidOperationException("Cannot run the next phase; no current phase");

        GameState nextState = new(this, Support);

        foreach (var player in Players) 
        {
            player.Controller.RunningPhase(phase, this);
        }
        GameState next = phase.Run(nextState);

        foreach (var player in Players) 
        {
            player.Controller.RanPhase(phase, this);
        }
        
        return next;
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
    public GameResult? GameResult { get; internal set; }
    public GameSetup Setup => _gameSetup;

    public void AddEvent(GameEvent newEvent, bool broadcastToController = true)
    {
        _events.Add(newEvent);

        if (newEvent is StartRoleClaimedEvent claim)
        {
            _claims.Add(claim);
        }

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

    public GameSlot GetSlot(Player player)
        => _slots[player.Name];
    
    public GameSlot GetSlot(string slotName)
        => _slots[slotName];

    public GameSlot this[string slotName] 
        => GetSlot(slotName);
    
    public override string ToString() 
        => $"{string.Join(",", PlayerSlots.Select(p => p.Role))}[{string.Join(",", CenterSlots.Select(p => p.Role))}]";

    internal void SendRolesToControllers()
    {
        // In order to avoid sending events from possible worlds to players, we only broadcast dealt events after a root state has been chosen
        foreach (var dealtEvent in _events.OfType<DealtCardEvent>())
        {
            dealtEvent.Player?.Controller.ObservedEvent(dealtEvent, this);
        }
    }
    
    public bool IsPossibleGivenEvents(IEnumerable<GameEvent> events) 
        => events.All(e => e.IsPossibleInGameState(this));

    public GameRole GetStartRole(GameSlot gameSlot)
        => GetStartRole(gameSlot.Name);
    
    public GameRole GetStartRole(string name) 
        => Root[name].Role;
    
    public GameRole GetStartRole(Player player) 
        => GetStartRole(player.Name);

    public GameState SwapRoles(string slot1, string slot2)
    {
        GameSlot[] slots = AllSlots.ToArray();
        
        GameSlot s1 = slots.First(s => s.Name == slot1);
        GameSlot s2 = slots.First(s => s.Name == slot2);

        GameRole role1 = s1.Role;
        GameRole role2 = s2.Role;

        GameSlot[] playerSlots = slots.Where(s => s.Player is not null)
            .Select(SlotMutator)
            .ToArray();

        GameSlot[] centerSlots = slots.Where(s => s.Player is null)
            .Select(SlotMutator)
            .ToArray();

        return new GameState(this, playerSlots, centerSlots);

        GameSlot SlotMutator(GameSlot s)
        {
            if (s == s1)
            {
                return new GameSlot(s.Name, role2, s.Player);
            } else if (s == s2)
            {
                return new GameSlot(s.Name, role1, s.Player);
            }

            return new GameSlot(s);
        }
    }

    public GameResult DetermineGameResults(IDictionary<Player, int> votes, int supportingClaims = 0)
    {
        int totalVotes = votes.Values.Sum();
        int skips = votes.Keys.Count - totalVotes;
        int maxVotes = votes.Values.Max();

        // Players with 1 vote won't die unless at least 1 person skips
        int minExecutionVotes = 2;
        if (skips > 0)
        {
            minExecutionVotes = 1;
        }
        
        IEnumerable<Player> dead = votes.Where(kvp => kvp.Value == maxVotes && kvp.Value >= minExecutionVotes)
            .Select(kvp => kvp.Key);

        return new GameResult(dead, this, votes, supportingClaims);
    }

    public int ObservedSupport(Player player)
    {
        IEnumerable<StartRoleClaimedEvent> claims = Events.OfType<StartRoleClaimedEvent>();
        
        return claims.Count(c => c.Player != player && c.IsClaimValidFor(this));
    }
}