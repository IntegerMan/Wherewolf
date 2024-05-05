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
            AddEvent(new DealtCardEvent(slot.StartRole, slot), false);
        }
        _allPhases = setup.Phases;
        _remainingPhases = new Queue<GamePhase>(_allPhases);
        Root = this;
        Support = support;
    }    

    internal GameState(GameState parentState, double support)
    {
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases.Skip(1));
        _gameSetup = parentState._gameSetup;
        _allPhases = parentState._allPhases.ToArray();
        _events.AddRange(parentState.Events);
        _centerSlots = parentState.CenterSlots.Select(c => new GameSlot(c)).ToArray();
        _playerSlots = parentState.PlayerSlots.Select(c => new GameSlot(c)).ToArray();
        Parent = parentState;
        Root = parentState.Root;
        Support = support;
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
        List<GameState> phasePermutations = _gameSetup.GetPermutationsAtPhase(CurrentPhase).ToList();
        if (!phasePermutations.Any())
            throw new InvalidOperationException("No phase permutations found for phase " + (CurrentPhase?.Name ?? "Voting") + " for player " + player.Name);
        
        /* For testing specific permutations...
        phasePermutations = phasePermutations.Where(p => p.State.GetSlot("Player").StartRole == GameRole.Robber)
                                             .Where(p => p.State.GetSlot("Target").StartRole == GameRole.Werewolf)
                                             .Where(p => p.State.GetSlot("Other").StartRole == GameRole.Villager)
                                             .Where(p => p.State.GetSlot("Center 1").StartRole == GameRole.Villager)
                                             .Where(p => p.State.GetSlot("Center 2").StartRole == GameRole.Villager)
                                             .Where(p => p.State.GetSlot("Center 3").StartRole == GameRole.Werewolf)
                                             .ToList();
        */
        
        List<GameState> validPermutations = phasePermutations.Where(p => p.IsPossibleGivenEvents(observedEvents)).ToList();
        if (!validPermutations.Any())
            throw new InvalidOperationException("No valid permutations found for phase " + (CurrentPhase?.Name ?? "Voting") + " for player " + player.Name);
        
        double startPopulation = validPermutations.Sum(p => p.Support);
        
        // Calculate starting role probabilities
        foreach (var slot in AllSlots)
        {
            foreach (var role in Roles.Distinct())
            {
                // Figure out the number of possible worlds where the slot had the role at the start
                double startRoleSupport = validPermutations.Where(p => p.GetSlot(slot.Name).StartRole == role)
                                              .Sum(p => p.Support);

                probabilities.RegisterStartRoleProbabilities(slot, role, startRoleSupport, startPopulation);
                
                // Figure out the number of possible worlds where the slot currently has the role
                double currentRoleSupport = validPermutations.Where(p => p.GetSlot(slot.Name).BeginningOfPhaseRole == role)
                    .Sum(p => p.Support);

                probabilities.RegisterCurrentRoleProbabilities(slot, role, currentRoleSupport, startPopulation);
                
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
        => $"{string.Join(",", PlayerSlots.Select(p => p.BeginningOfPhaseRole))}[{string.Join(",", CenterSlots.Select(p => p.BeginningOfPhaseRole))}]";

    internal void SwapRoles(GameSlot slot1, GameSlot slot2)
    {
        if (slot1.BeginningOfPhaseRole != slot1.EndOfPhaseRole)
            throw new InvalidOperationException("Swapping roles encountered a slot1 that was already swapped");
        if (slot2.BeginningOfPhaseRole != slot2.EndOfPhaseRole)
            throw new InvalidOperationException("Swapping roles encountered a slot2 that was already swapped");
        
        slot1.EndOfPhaseRole = slot2.BeginningOfPhaseRole;
        slot2.EndOfPhaseRole = slot1.BeginningOfPhaseRole;
    }

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
}