using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameState
{
    private readonly GameSetup _gameSetup;
    private readonly GameSlot[] _centerSlots;
    private readonly GameSlot[] _playerSlots;
    private readonly Queue<GamePhase> _remainingPhases;
    private readonly List<GameEvent> _events = new();
    public double Support { get; }
    
    internal GameState(GameSetup setup, IReadOnlyList<GameRole> shuffledRoles, double support)
    {
        setup.Validate();
        _gameSetup = setup;

        _playerSlots = BuildPlayerSlots(setup.Players, shuffledRoles);
        _centerSlots = BuildCenterSlots(setup.Players, shuffledRoles);

        AssignOrderIndexToEachPlayer(setup.Players);
        foreach (var slot in AllSlots)
        {
            AddEvent(new DealtCardEvent(slot.Role, slot), false);
        }
        _remainingPhases = new Queue<GamePhase>(setup.Phases);
        Root = this;
        Support = support;
    }    

    internal GameState(GameState parentState, double support)
    {
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases.Skip(1));
        _gameSetup = parentState._gameSetup;
        _events.AddRange(parentState.Events);
        _centerSlots = parentState.CenterSlots.Select(c => new GameSlot(c)).ToArray();
        _playerSlots = parentState.PlayerSlots.Select(c => new GameSlot(c)).ToArray();
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
        _centerSlots = centerSlots.ToArray();
        _playerSlots = playerSlots.ToArray();
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
        _playerSlots = BuildPlayerSlots(setup.Players, setup.Roles.ToList());
        _centerSlots = BuildCenterSlots(setup.Players, setup.Roles.ToList());
        
        Parent = null;
        Root = this;
        Support = 1;
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
        List<GameState> validPermutations = GetPossibleGameStatesForPlayer(player);

        double startPopulation = validPermutations.Sum(p => p.Support);
        
        // Calculate starting role probabilities
        foreach (var slot in AllSlots)
        {
            foreach (var role in Roles.Distinct())
            {
                // Figure out the number of possible worlds where the slot had the role at the start
                double startRoleSupport = validPermutations.Where(p => p.Root[slot.Name].Role == role)
                                              .Sum(p => p.Support);

                probabilities.RegisterStartRoleProbabilities(slot, role, startRoleSupport, startPopulation);
                
                // Figure out the number of possible worlds where the slot currently has the role
                double currentRoleSupport = validPermutations.Where(p => p[slot.Name].Role == role)
                    .Sum(p => p.Support);

                probabilities.RegisterCurrentRoleProbabilities(slot, role, currentRoleSupport, startPopulation);
            }
        }
        
        return probabilities;
    }

    private List<GameState> GetPossibleGameStatesForPlayer(Player player)
    {
        List<GameEvent> observedEvents = Events.Where(e => e.IsObservedBy(player)).ToList();
        List<GameState> phasePermutations = _gameSetup.GetPermutationsAtPhase(CurrentPhase).ToList();
        if (!phasePermutations.Any())
            throw new InvalidOperationException("No phase permutations found for phase " + (CurrentPhase?.Name ?? "Voting") + " for player " + player.Name);
        
        List<GameState> validPermutations = phasePermutations.Where(p => p.IsPossibleGivenEvents(observedEvents)).ToList();
        if (!validPermutations.Any())
            throw new InvalidOperationException("No valid permutations found for phase " + (CurrentPhase?.Name ?? "Voting") + " for player " + player.Name);
        return validPermutations;
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
        if (CurrentPhase is VotingPhase)
        {
            return this;
        }

        GameState nextState = RunNext();
        return nextState.RunToEndOfNight();
    }    

    private GameState RunNext()
    {
        if (CurrentPhase is null)
            throw new InvalidOperationException("Cannot run the next phase; no current phase");

        GameState nextState = new(this, Support);

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
    public GameResult? GameResult { get; internal set; }

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

    public GameSlot this[string slotName] => GetSlot(slotName);
    
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

    public GameResult DetermineGameResults(Dictionary<Player, Player> votes) 
        => DetermineGameResults(GameState.GetVotingResults(votes));

    public GameResult DetermineGameResults(Dictionary<Player, int> votes)
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

        return new GameResult(dead, this);
    }

    public Dictionary<Player, float> GetVoteVictoryProbabilities(Player player)
    {
        List<Dictionary<Player, Player>> permutations = _gameSetup.GetVotingPermutations().ToList();

        // Build a collection of results based on who the player voted for - for every world this player thinks might be valid
        Dictionary<Player, List<GameResult>> results = new();
        foreach (var state in GetPossibleGameStatesForPlayer(player))
        {
            state.AddGameStateVoteResultPossibilities(player, permutations, results);
        }

        // Calculate win % for each
        Dictionary<Player, float> winProbability = new();
        foreach (var kvp in results)
        {
            winProbability[kvp.Key] = kvp.Value.Average(r => r.WinningPlayers.Contains(player) ? 1f : 0f);
        }

        return winProbability;
    }
    
    public static Dictionary<Player, int> GetVotingResults(Dictionary<Player, Player> votes)
    {
        // TODO: This will probably need to be revisited to support the Hunter / Bodyguard

        Dictionary<Player, int> voteTotals = new();
        
        // Initialize everyone at 0 votes. This ensures they're in the dictionary
        foreach (var player in votes.Keys)
        {
            voteTotals[player] = 0;
        }

        // Tabulate votes
        foreach (var target in votes.Values)
        {
            voteTotals[target]++;
        }
        
        return voteTotals;
    }

    private void AddGameStateVoteResultPossibilities(Player player, IEnumerable<Dictionary<Player, Player>> permutations, Dictionary<Player, List<GameResult>> results)
    {
        foreach (var perm in permutations)
        {
            Dictionary<Player, int> votes = GetVotingResults(perm);
            
            GameResult gameResult = DetermineGameResults(votes);

            Player action = perm[player];
            if (!results.ContainsKey(action))
            {
                results[action] = [gameResult];
            }
            else
            {
                results[action].Add(gameResult);
            }
        }
    }
}