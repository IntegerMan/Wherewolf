using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MoreLinq;

namespace MattEland.Wherewolf;

public class GameSetup
{
    private readonly List<GamePhase> _phases = new();
    private readonly List<Player> _players = new();
    private readonly List<GameRole> _roles = new();
    private readonly Dictionary<string, List<GameState>> _phasePermutations = new();
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

    public void AddPlayer(Player player)
    {
        if (_players.Contains(player))
            throw new InvalidOperationException("Player has already been added");

        _players.Add(player);
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
        slotShuffler ??= new RandomShuffler();

        // Pre-calculate all phases
        CalculatePhases();

        // Pre-calculate all permutations
        CalculatePermutations();

        // Find a game state from our permutations (we want to avoid instantiating the same state again)
        List<GameRole> shuffledRoles = slotShuffler.Shuffle(Roles).ToList();
        string rolesList = shuffledRoles.ToDelimitedString(",");
        GameState permutation = GetPermutationsAtPhase(_phases.First())
            .First(p => p.AllSlots.Select(s => s.Role).ToDelimitedString(",") == rolesList);

        permutation.SendRolesToControllers();

        return permutation;
    }

    private void CalculatePhases()
    {
        List<GamePhase> phases = new()
        {
            new SetupNightPhase() // This diagnostic phase should always be present
        };
        
        foreach (var nightPhaseType in Roles.Distinct().SelectMany(r => r.GetNightPhasesForRole()))
        {
            phases.Add((GamePhase)Activator.CreateInstance(nightPhaseType)!);
        }

        _phases.Clear();
        _phases.AddRange(phases.OrderBy(p => p.Order));
    }

    internal void Validate()
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
    }

    private void CalculatePermutations()
    {
        _phasePermutations.Clear();
        
        BuildSetupPermutations();

        List<GameState> currentPhasePermutations = [.._phasePermutations["Setup"]];
        string nextPhase = currentPhasePermutations.First().Phases.Skip(1).FirstOrDefault()?.Name ?? "Voting";

        BuildPermutationsForNextPhase(currentPhasePermutations, nextPhase);
    }

    private void BuildPermutationsForNextPhase(List<GameState> priorStates, string phaseName)
    {
        List<GameState> possibleStates = priorStates.SelectMany(p => p.PossibleNextStates).ToList();
        if (!possibleStates.Any())
        {
            _phasePermutations["Voting"] = priorStates.ToList();
            return;
        }
        
        string nextPhaseName = possibleStates.First().CurrentPhase?.Name ?? "Voting";

        if (phaseName == "Robber")
        {
            Console.Write("Robble");
        }
        _phasePermutations[phaseName] = possibleStates;
        
        BuildPermutationsForNextPhase(possibleStates, nextPhaseName);
    }

    private void BuildSetupPermutations()
    {
        // Generate the unique combinations of each role
        IEnumerable<IList<GameRole>> permutations = _roles.Permutations();

        // Break our permutations into groups based on role combinations. This helps merge duplicate permutations
        _phasePermutations["Setup"] = new List<GameState>();
        foreach (var group in permutations.GroupBy(p => string.Join(",", p)))
        {
            // Represent multiple similar states merged together using the Support property to indicate merged probabilities
            GameState state = new(this, group.First().ToList(), group.Count());
            _phasePermutations["Setup"].Add(state);
        }
    }

    public IEnumerable<GameState> GetPermutationsAtPhase(GamePhase? currentPhase)
    {
        if (!_phasePermutations.Any())
        {
            CalculatePermutations();
        }

        return _phasePermutations[currentPhase?.Name ?? "Voting"];
    }
}