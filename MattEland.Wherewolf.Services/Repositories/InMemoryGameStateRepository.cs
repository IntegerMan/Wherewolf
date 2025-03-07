namespace MattEland.Wherewolf.Services.Repositories;

public class InMemoryGameStateRepository : IGameStateRepository
{
    private readonly Dictionary<Guid, GameState> _states = new();
    
    public Guid StoreNewGame(GameState state)
    {
        Guid id = Guid.CreateVersion7();
        _states[id] = state;
        return id;
    }
    
    public GameState? FindGame(Guid id) => _states.GetValueOrDefault(id);
}