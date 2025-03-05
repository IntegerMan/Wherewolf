namespace MattEland.Wherewolf.BlazorFrontEnd.Repositories;

public interface IGameStateRepository
{
    Guid StoreNewGame(GameState state);
    GameState? FindGame(Guid id);
}