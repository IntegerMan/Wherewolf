using System.Text;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf;

public class GameState
{
    private readonly Queue<GamePhase> _remainingPhases;
    private readonly List<GameEvent> _events = [];
    private readonly List<SocialEvent> _claims = [];
    private readonly Dictionary<string, GameSlot> _slots = new();
    public double Support { get; internal set; }
    public GameSetup Setup { get; }
    
    public GameState(GameSetup setup, IReadOnlyList<GameRole> shuffledRoles, double support)
    {
        Setup = setup;
        PlayerSlots = BuildPlayerSlots(setup.Players, shuffledRoles);
        CenterSlots = BuildCenterSlots(setup.Players, shuffledRoles);
        foreach(var slot in AllSlots)
        {
            _slots[slot.Name] = slot;
            AddEvent(EventPool.DealtCardEvent(slot.Name, slot.Role), broadcastToController: false);
        }

        _remainingPhases = new Queue<GamePhase>(setup.Phases);
        Root = this;
        Support = support;
    }    

    internal GameState(GameState parentState, double support)
    {
        Setup = parentState.Setup;
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases.Skip(1));
        CenterSlots = parentState.CenterSlots.Select(c => new GameSlot(c)).ToArray();
        PlayerSlots = parentState.PlayerSlots.Select(c => new GameSlot(c)).ToArray();
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
    private GameState(GameState parentState, IEnumerable<GameSlot> playerSlots, IEnumerable<GameSlot> centerSlots)
    {
        Setup = parentState.Setup;
        _remainingPhases = new Queue<GamePhase>(parentState._remainingPhases);
        CenterSlots = centerSlots.ToArray();
        PlayerSlots = playerSlots.ToArray();
        
        foreach(var slot in AllSlots)
        {
            _slots[slot.Name] = slot;
        }
        Parent = parentState.Parent;
        Root = parentState.Root;
        Support = parentState.Support;
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


    public GameSlot[] PlayerSlots { get; }

    public GameSlot[] CenterSlots { get; }

    public IEnumerable<GameSlot> AllSlots => [..PlayerSlots, ..CenterSlots];

    public IEnumerable<Player> Players => Setup.Players;
    public IEnumerable<GameRole> Roles => Setup.Roles;
    public int GetRoleCount(GameRole role) => Roles.Count(r => r == role);
    public IEnumerable<GameEvent> Events
    {
        get
        {
            if (Parent is not null)
            {
                foreach (var e in Parent.Events)
                {
                    yield return e;
                }
            }

            for (int i = 0; i < _events.Count; i++)
            {
                yield return _events[i];
            }
        }
    }
    
    
    public IEnumerable<SocialEvent> Claims
    {
        get
        {
            if (Parent is not null)
            {
                foreach (var c in Parent.Claims)
                {
                    yield return c;
                }
            }

            for (int index = 0; index < _claims.Count; index++)
            {
                yield return _claims[index];
            }
        }
    }

    public PlayerProbabilities CalculateProbabilities(Player player)
    {
        PlayerProbabilities probabilities = new();

        // Start with all permutations
        GameState[] validPermutations = VotingHelper.GetPossibleGameStatesForPlayer(player, this).ToArray();

        double startPopulation = validPermutations.Sum(p => p.Support);

        StartRoleClaimedEvent[] claimedEvents = Claims.OfType<StartRoleClaimedEvent>().ToArray();

        // Materialise once to avoid re-allocating the combined array per iteration.
        GameSlot[] allSlots = AllSlots.ToArray();
        GameRole[] distinctRoles = Roles.Distinct().ToArray();

        // Build per-(slot, role) support totals in a single pass over validPermutations
        // instead of re-scanning for every (slot × role) pair in the nested loop below.
        Dictionary<(string slotName, GameRole role), double> startSupportTotals = new();
        Dictionary<(string slotName, GameRole role), double> currentSupportTotals = new();

        foreach (GameState perm in validPermutations)
        {
            foreach (GameSlot slot in allSlots)
            {
                (string, GameRole) startKey = (slot.Name, perm.Root[slot.Name].Role);
                startSupportTotals[startKey] = startSupportTotals.GetValueOrDefault(startKey) + perm.Support;

                (string, GameRole) currentKey = (slot.Name, perm[slot.Name].Role);
                currentSupportTotals[currentKey] = currentSupportTotals.GetValueOrDefault(currentKey) + perm.Support;
            }
        }

        // Calculate role probabilities using O(1) dictionary lookups per (slot, role) pair.
        foreach (GameSlot slot in allSlots)
        {
            foreach (GameRole role in distinctRoles)
            {
                double startRoleSupport = startSupportTotals.GetValueOrDefault((slot.Name, role));

                IEnumerable<Player> startSupport = claimedEvents
                    .Where(e => e.ClaimedRole == role && e.Player == slot.Player && e.IsClaimValidFor(this))
                    .Select(e => e.Player)
                    .Where(e => e != player)
                    .Distinct();

                probabilities.RegisterStartRoleProbabilities(slot, role, startRoleSupport, startPopulation, startSupport);

                double currentRoleSupport = currentSupportTotals.GetValueOrDefault((slot.Name, role));

                probabilities.RegisterCurrentRoleProbabilities(slot, role, currentRoleSupport, startPopulation, []);
            }
        }

        return probabilities;
    }

    public void RunNext(Action<GameState> callback)
    {
        GamePhase? phase = CurrentPhase;
        if (phase is null) throw new InvalidOperationException("Cannot run the next phase; no current phase");

        GameState nextState = new(this, Support);
        
        phase.Run(nextState, callback);
    }

    public bool IsGameOver => _remainingPhases.Count == 0;
    public GamePhase? CurrentPhase => IsGameOver ? null : _remainingPhases.Peek();
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

    internal void AddEvent(GameEvent newEvent, bool broadcastToController = true)
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

    internal void AddEvent(SocialEvent newEvent)
    {
        _claims.Add(newEvent);
    }

    public GameSlot GetSlot(Player player)
        => _slots[player.Name];

    public GameSlot this[string slotName] 
        => _slots[slotName];
    
    public override string ToString()
    {
        StringBuilder sb = new( $"{string.Join(",", PlayerSlots.Select(p => p.Role))}[{string.Join(",", CenterSlots.Select(p => p.Role))}] {CurrentPhase?.Name}" );
        
        foreach (var e in _events)
        {
            sb.AppendLine(e.ToString());
        }
        
        return sb.ToString();
    }

    internal void SendRolesToControllers()
    {
        // In order to avoid sending events from possible worlds to players, we only broadcast dealt events after a root state has been chosen
        foreach (var dealtEvent in _events.OfType<DealtCardEvent>())
        {
            _slots[dealtEvent.SlotName].Player!.Controller.ObservedEvent(dealtEvent, this);
        }
    }
    
    public bool IsPossibleGivenEvents(GameEvent[] events)
    {
        for (var i = 0; i < events.Length; i++)
        {
            if (!events[i].IsPossibleInGameState(this))
            {
                return false;
            }
        }

        return true;
    }

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
            }

            if (s == s2)
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

        return new GameResult(dead, this, votes);
    }

    public SpecificRoleClaim[] GeneratePossibleSpecificRoleClaims(Player player)
    {
        // TODO: It might make more sense and keep scope narrower to restrict to specific claims along their prior claim
        // This way they couldn't claim role 1 and then role 2 with more specifics - though changing minds in some cases
        // can be cool - like a WW outing as a WW if they think they've been robbed.

        List<SpecificRoleClaim> candidates = new();
        foreach (var role in Roles.Distinct())
        {
            candidates.AddRange(role.GetPossibleSpecificRoleClaims(player, this));
        }

        // Cheap combinatorial gate: drop claims that are provably impossible based on role-card counts alone
        // (e.g. Robber claiming to have stolen the Robber role when only one Robber card exists).
        // TODO: This will need to be modified / removed when role swapping roles like troublemaker come in        
        List<SpecificRoleClaim> survivors = candidates
            .Where(c => c.IsCombinatoriallyPossible(this))
            .ToList();

        if (survivors.Count == 0) return [];

        // Rigorous check: keep only claims that hold true in at least one permutation of
        // the game's role assignments, confirming the claim is possible in some game state.
        GameState[] permutations = Setup.GetPermutationsAtPhase(CurrentPhase).ToArray();
        return survivors
            .Where(c => permutations.Any(p => c.IsClaimValidFor(p)))
            .ToArray();
    }

    public bool ContainsEvent(GameEvent e) => Events.Contains(e);
}