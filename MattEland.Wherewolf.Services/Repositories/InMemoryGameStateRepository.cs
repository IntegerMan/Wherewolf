namespace MattEland.Wherewolf.Services.Repositories;

public class InMemoryGameStateRepository : IGameStateRepository
{
    private readonly Dictionary<Guid, GameManager> _games = new();
    
    public Guid StoreNewGame(GameManager game)
    {
        Guid id = Guid.CreateVersion7();
        _games[id] = game;
        return id;
    }
    
    public GameManager? FindGame(Guid id) => _games.GetValueOrDefault(id);
}