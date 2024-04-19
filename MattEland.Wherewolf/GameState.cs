using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameState
{
    private readonly GameRole[] _roles;
    private readonly Player[] _players;
    private readonly GameSlot[] _centerSlots;
    private readonly GameSlot[] _playerSlots;

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

        int i = 0;
        List<GameRole> shuffledRoles = shuffler.Shuffle(roles).ToList();

        _playerSlots = players.Select(p => new GameSlot(p.Name, shuffledRoles[i++]) { Player = p}).ToArray();

        int c = 1;
        _centerSlots = shuffledRoles.Skip(players.Count()).Select(r => new GameSlot("Center " + (c++), r)).ToArray();
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
}

public interface ISlotShuffler
{
    IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles);
}