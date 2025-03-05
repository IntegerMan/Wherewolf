using MattEland.Wherewolf.BlazorFrontEnd.Repositories;
using MattEland.Wherewolf.Events;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class GamePage(IGameStateRepository repo) : ComponentBase
{
    [Parameter]
    public Guid GameId { get; set; }
    
    public GameState? Game { get; set; }
    
    public Player? PerspectivePlayer { get; set; }

    public bool CanAdvance => Game is { IsGameOver: false };
    public bool IsGameOver => Game is { IsGameOver: true };
    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Game = repo.FindGame(GameId);
        PerspectivePlayer = Game?.Players.First();
    }

    public string NewGameUrl => "/setup";

    private void AdvanceToNextPhase() => Game = Game?.RunNext();
    private void AdvanceToEnd() => Game = Game?.RunToEnd();
}