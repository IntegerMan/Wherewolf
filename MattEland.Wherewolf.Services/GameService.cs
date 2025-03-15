using MattEland.Wherewolf.Services.Repositories;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Services;

public class GameService(IGameStateRepository stateRepository)
{
    public Guid StartGame(GameSetup setup)
    {
        GameManager game = new(setup);
        
        return stateRepository.StoreNewGame(game);
    }
}