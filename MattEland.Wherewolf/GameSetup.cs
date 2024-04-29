using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;
using MoreLinq;

namespace MattEland.Wherewolf;

public class GameSetup
{
    private readonly List<GamePhase> _phases = new();
    private readonly List<Player> _players = new();
    private readonly List<GameRole> _roles = new();
    private readonly Dictionary<string, List<GamePermutation>> _phasePermutations = new();
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
        GamePermutation permutation = GetPermutationsAtPhase(_phases.FirstOrDefault())
            .First(p => p.State.AllSlots.Select(s => s.BeginningOfPhaseRole).ToDelimitedString(",") == rolesList);

        permutation.State.SendRolesToControllers();

        return permutation.State;
    }

    private void CalculatePhases()
    {
        List<GamePhase> phases = new();
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

        List<GamePermutation> currentPhasePermutations = new();

        // Generate the unique combinations of each role
        IEnumerable<IList<GameRole>> permutations = _roles.Permutations();

        // Break our permutations into groups based on role combinations. This helps merge duplicate permutations
        foreach (var group in permutations.GroupBy(p => string.Join(",", p)))
        {
            // Represent multiple similar states merged together using the Support property to indicate merged probabilities
            GameState state = new(this, group.First().ToList());

            string phaseName = state.CurrentPhase?.Name ?? "Voting";
            if (!_phasePermutations.ContainsKey(phaseName))
            {
                _phasePermutations[phaseName] = new List<GamePermutation>();
            }

            GamePermutation gamePermutation = new(state, support: group.Count());
            _phasePermutations[phaseName].Add(gamePermutation);
            currentPhasePermutations.Add(gamePermutation);
        }

        // Now extrapolate future phases
        while (!currentPhasePermutations.First().State.IsGameOver)
        {
            List<GamePermutation> priorPhasePermutations = currentPhasePermutations.ToList();
            currentPhasePermutations.Clear();

            foreach (var priorPermutation in priorPhasePermutations)
            {
                List<GameState> possibleStates = priorPermutation.State.PossibleNextStates.ToList();
                int count = possibleStates.Count;
                foreach (var state in possibleStates)
                {
                    GamePermutation permutation = new(state, priorPermutation.Support * (1d / count));
                    currentPhasePermutations.Add(permutation);

                    string phaseName = permutation.State.CurrentPhase?.Name ?? "Voting";

                    if (phaseName == "Robber")
                    {
                        Console.WriteLine("Yo");
                    }

                    if (!_phasePermutations.ContainsKey(phaseName))
                    {
                        _phasePermutations[phaseName] = new List<GamePermutation>();
                    }

                    _phasePermutations[phaseName].Add(permutation);
                }
            }
        }
    }

    public IEnumerable<GamePermutation> GetPermutationsAtPhase(GamePhase? currentPhase)
    {
        if (!_phasePermutations.Any())
        {
            CalculatePermutations();
        }

        return _phasePermutations[currentPhase?.Name ?? "Voting"];
    }
}