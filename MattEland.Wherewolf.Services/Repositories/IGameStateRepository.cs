namespace MattEland.Wherewolf.Services.Repositories;

public interface IGameStateRepository
{
    Guid StoreNewGame(GameManager game);
    GameManager? FindGame(Guid id);
}