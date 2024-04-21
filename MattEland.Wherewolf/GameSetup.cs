using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameSetup
{
    private readonly List<Player> _players = new();
    private readonly List<GameRole> _roles = new();
    public IEnumerable<Player> Players => _players.AsReadOnly();
    public IEnumerable<GameRole> Roles => _roles.AsReadOnly();

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

    public void AddRole(GameRole role)
    {
        if (_roles.Contains(role)) 
            throw new InvalidOperationException("Role has already been added. If you want multiple of the same role, instantiate multiple copies.");
        
        _roles.Add(role);
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

        GameState startGame = new GameState(this, slotShuffler);
        
        return startGame;
    }

    internal void Validate()
    {
        if (_players.Count + 3 != _roles.Count)
        {
            throw new InvalidOperationException($"There must be exactly 3 more roles allocated to the game than players. Roles: {_roles.Count}, Players: {_players.Count}");
        }
        if (_players.Count < 3)
        {
            throw new InvalidOperationException($"There must be at least 3 players in the game, Players: {_players.Count}");
        }
        if (_roles.Select(r => r.Team).Distinct().Count() == 1)
        {
            throw new InvalidOperationException("All roles were on the same team");
        }
    }
}