namespace MattEland.Wherewolf.Services.Repositories;

public interface IGameStateRepository
{
    Guid StoreNewGame(GameState state);
    GameState? FindGame(Guid id);
}