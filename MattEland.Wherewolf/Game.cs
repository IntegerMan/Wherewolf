using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class Game
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
        
        return new GameState(this.Players, this.Roles, slotShuffler);
    }
}