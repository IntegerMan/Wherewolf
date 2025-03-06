using MattEland.Wherewolf.BlazorFrontEnd.Repositories;
using MattEland.Wherewolf.Probability;
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
        UpdateProbabilities();
    }

    private void UpdateProbabilities()
    {
        if (Game is not null && PerspectivePlayer is not null)
        {
            PlayerProbabilities = Game?.CalculateProbabilities(PerspectivePlayer);
        }
    }

    public string NewGameUrl => "/setup";

    private void AdvanceToNextPhase()
    {
        Game?.RunNext(game => InvokeAsync(() => { 
            Game = game;
            UpdateProbabilities();
            StateHasChanged();
        }));
    }

    public PlayerProbabilities? PlayerProbabilities { get; set; }

    private void AdvanceToEnd()
    {
        Game?.RunToEnd(game => InvokeAsync(() => { 
            Game = game;
            UpdateProbabilities();
            StateHasChanged();
        }));
    }
}