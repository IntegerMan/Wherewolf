using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameState
{
    private readonly GameRole[] _roles;
    private readonly Player[] _players;
    private readonly GameSlot[] _centerSlots;
    private readonly GameSlot[] _playerSlots;
    private readonly List<GameEvent> _events = new();

    public GameState(IEnumerable<Player> players, IEnumerable<GameRole> roles, ISlotShuffler shuffler)
    {
        // Validation
        if (players.Count() + 3 != roles.Count())
        {
            throw new InvalidOperationException($"There must be exactly 3 more roles allocated to the game than players. Roles: {roles.Count()}, Players: {players.Count()}");
        }
        if (players.Count() < 3)
        {
            throw new InvalidOperationException($"There must be at least 3 players in the game, Players: {players.Count()}");
        }

        if (roles.Select(r => r.Team).Distinct().Count() == 1)
        {
            throw new InvalidOperationException("All roles were on the same team");
        }
        
        _roles = roles.ToArray();
        _players = players.ToArray();

        List<GameRole> shuffledRoles = shuffler.Shuffle(roles).ToList();

        _playerSlots = BuildPlayerSlots(players, shuffledRoles);
        _centerSlots = BuildCenterSlots(players, shuffledRoles);

        AssignOrderIndexToEachPlayer(players);
        RegisterStartingCards();
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
            _events.Add(new DealtCardEvent(slot.StartRole, slot));
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

    public IEnumerable<Player> Players => _players;
    public IEnumerable<GameRole> Roles => _roles;
    public IEnumerable<GameEvent> Events => _events.AsReadOnly();

    public PlayerState GetPlayerStates(Player player)
    {
        PlayerState state = new(player);
        
        state.AddEvents(_events.Where(e => e.IsObservedBy(player)));

        return state;
    }
}

public interface ISlotShuffler
{
    IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles);
}