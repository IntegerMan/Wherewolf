namespace MattEland.Wherewolf;

public class Game
{
    private readonly List<Player> _players = new();
    public IEnumerable<Player> Players => _players.AsReadOnly();

    public void AddPlayer(Player player)
    {
        if (_players.Contains(player)) throw new InvalidOperationException("Player has already been added");
        
        _players.Add(player);
    }
}