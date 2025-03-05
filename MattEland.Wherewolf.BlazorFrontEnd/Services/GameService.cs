using MattEland.Wherewolf.BlazorFrontEnd.Repositories;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.BlazorFrontEnd.Services;

public class GameService(IGameStateRepository stateRepository)
{
    public Guid StartGame(GameSetup setup)
    {
        GameState state = setup.StartGame();
        
        return stateRepository.StoreNewGame(state);
    }
}