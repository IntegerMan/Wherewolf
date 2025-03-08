using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Client;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Services.Repositories;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class GamePage : ComponentBase, 
    IRecipient<ChangeClientModeMessage>,
    IRecipient<VoteRequestedMessage>
{
    private readonly IGameStateRepository _repo;

    public GamePage(IGameStateRepository repo)
    {
        _repo = repo;
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    [Parameter]
    public Guid GameId { get; set; }
    
    public ClientMode Mode { get; set; } = ClientMode.Normal;
    
    public GameState? Game { get; set; }
    
    public Player? PerspectivePlayer { get; set; }

    public bool CanAdvance => Game is { IsGameOver: false };
    public bool IsGameOver => Game is { IsGameOver: true };
    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Game = _repo.FindGame(GameId);
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

    public void Receive(ChangeClientModeMessage message)
    {
        Mode = message.Mode;
        StateHasChanged();
    }

    public void Receive(VoteRequestedMessage message) 
    {
        Mode = ClientMode.Vote;
        PlayerProbabilities = message.PlayerProbabilities;
        StateHasChanged();
    }
}