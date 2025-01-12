using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MoreLinq;

namespace MattEland.Wherewolf.Setup;

public class GameSetup
{
    private readonly List<GamePhase> _phases = new();
    private readonly List<Player> _players = new();
    private readonly List<GameRole> _roles = new();
    private VotePermutationsProvider? _votePermutations;
    private GameState? _root;
    private GameState[] _possibleRoots = [];
    public IEnumerable<Player> Players => _players.AsReadOnly();
    public IEnumerable<GameRole> Roles => _roles.AsReadOnly();
    public GamePhase[] Phases
    {
        get
        {
            if (!_phases.Any())
            {
                CalculatePhases();
            }

            return _phases.ToArray();
        }
    }

    public IDictionary<Player, Player>[] VotingPermutations => _votePermutations!.VotingPermutations;

    public void AddPlayer(Player player)
    {
        if (_players.Contains(player))
            throw new InvalidOperationException("Player has already been added");

        _players.Add(player);
        _votePermutations = new VotePermutationsProvider(_players);
    }

    public void AddPlayers(params Player[] players)
    {
        foreach (var player in players)
        {
            AddPlayer(player);
        }
    }

    public void AddRole(GameRole role, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            _roles.Add(role);
        }
    }

    public void AddRoles(params GameRole[] roles)
    {
        foreach (var role in roles)
        {
            AddRole(role);
        }
    }

    public GameState StartGame(ISlotShuffler? slotShuffler = null)
    {
        Validate();
        
        slotShuffler ??= new RandomShuffler();

        // Pre-calculate all phases
        CalculatePhases();

        // Find a game state from our permutations (we want to avoid instantiating the same state again)
        GameRole[] shuffledRoles = slotShuffler.Shuffle(Roles).ToArray();
        
        // Pre-calculate all permutations
        IEnumerable<IList<GameRole>> rolePermutations = shuffledRoles.Permutations();
        List<GameState> possibleStates = rolePermutations
            .Select(roles => new GameState(this, roles.ToArray(), support: 1))
            .ToList();
        
        // Find states with identical role allocations
        IEnumerable<IGrouping<string, GameState>> groupedRoles = possibleStates.GroupBy(s => string.Join(",", s.AllSlots.Select(sl => sl.Role)));
        _possibleRoots = groupedRoles.Select(g =>
        {
            var first = g.First();
            first.Support = g.Count();
            return first;
        }).ToArray();
        _root = _possibleRoots.First(s => s.AllSlots.Select(sl => sl.Role).SequenceEqual(shuffledRoles));
        
        // TODO: This seems like something that should live in a phase
        _root.SendRolesToControllers();

        return _root;
    }

    private void CalculatePhases()
    {
        List<GamePhase> phases =
        [
            new SetupNightPhase(), // This diagnostic phase should always be present
            new WakeUpPhase(),
            new VotingPhase()
        ];
        
        // Each player gets their on role claim phase
        foreach (var player in Players)
        {
            phases.Add(new InitialRoleClaimPhase(player));
        }
        
        foreach (var nightPhaseType in Roles.Distinct().SelectMany(r => r.GetNightPhasesForRole()))
        {
            phases.Add((GamePhase)Activator.CreateInstance(nightPhaseType)!);
        }

        _phases.Clear();
        _phases.AddRange(phases.OrderBy(p => p.Order));
        
        // Link backwards
        GamePhase? priorPhase = null;
        foreach (var phase in _phases)
        {
            phase.PriorPhase = priorPhase;
            priorPhase = phase;
        }
    }

    private void Validate()
    {
        if (_players.Count + 3 != _roles.Count)
        {
            throw new InvalidOperationException(
                $"There must be exactly 3 more roles allocated to the game than players. Roles: {_roles.Count}, Players: {_players.Count}");
        }

        if (_players.Count < 3)
        {
            throw new InvalidOperationException(
                $"There must be at least 3 players in the game, Players: {_players.Count}");
        }

        if (_roles.Select(r => r.GetTeam()).Distinct().Count() == 1)
        {
            throw new InvalidOperationException("All roles were on the same team");
        }
        
        AssignOrderIndexToEachPlayer(_players);
    }
    
        
    private static void AssignOrderIndexToEachPlayer(IEnumerable<Player> players)
    {
        int order = 0;
        foreach (var player in players)
        {
            player.Order = order++;
        }
    }
    
    public IEnumerable<GameState> GetPermutationsAtPhase(GamePhase? currentPhase)
    {
        // Walk down the tree of permutations to find the current phase
        List<GameState> currentBand = _possibleRoots.ToList();
        List<GameState> nextBand = [];
        
        while (currentBand.Count > 0)
        {
            foreach (var state in currentBand)
            {
                if (state.CurrentPhase == currentPhase)
                {
                    yield return state;
                }
                
                nextBand.AddRange(state.PossibleNextStates);
            }
            
            currentBand = nextBand;
            nextBand = [];
        }
    }
    
    public IEnumerable<GameState> PossibleRoots => _possibleRoots;
}