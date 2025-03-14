using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Client;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Services.Repositories;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class GamePage : ComponentBase, 
    IRecipient<ChangeClientModeMessage>,
    IRecipient<VoteRequestedMessage>,
    IRecipient<SpecificRoleClaimNeededMessage>
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
    
    public GameManager? Game { get; set; }
    
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
            PlayerProbabilities = Game?.CurrentState.CalculateProbabilities(PerspectivePlayer);
        }
    }

    public string NewGameUrl => "/setup";

    private void AdvanceToNextPhase()
    {
        Game!.RunNext();
        UpdateProbabilities();
        StateHasChanged();
    }

    public PlayerProbabilities? PlayerProbabilities { get; set; }
    public SpecificRoleClaimNeededMessage? SpecificRoleClaimNeededMessage { get; set; }

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

    public void Receive(SpecificRoleClaimNeededMessage message)
    {
        Mode = ClientMode.SpecificRoleClaim;
        SpecificRoleClaimNeededMessage = message;
        StateHasChanged();
    }
}