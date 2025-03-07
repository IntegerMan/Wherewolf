using MattEland.Wherewolf.Roles;
using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Client;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.BlazorFrontEnd.Services;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Setup;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class ConfigureGamePage : IRecipient<SetupRoleChangedMessage>
{
    private readonly GameService _gameService;
    private readonly NavigationManager _nav;
    private readonly PlayerWebController _playerController;

    public int PlayerCount { get; set; }

    public ConfigureGamePage(GameService gameService, NavigationManager nav, PlayerWebController playerController)
    {
        _gameService = gameService;
        _nav = nav;
        _playerController = playerController;
        PlayerCount = 3;
        Roles = [GameRole.Werewolf, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Robber, GameRole.Insomniac];
        GrowRolesAsNeeded();
        
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public List<GameRole?> Roles { get; }

    public GameRole[] AssignedRoles 
        => Roles.Take(PlayerCount + 3)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToArray();

    public string[] TickLabels 
        => Enumerable.Range(3, 10).Select(i => i.ToString()).ToArray();

    public bool AllRolesAssigned => true;

    public string RolesDebugString 
        => string.Join(", ", AssignedRoles.Select(r => r.ToString()));

    public bool IsValid => AllRolesAssigned;
    
    public int WerewolfTeamCount
        => AssignedRoles.Count(r => r.GetTeam() == Team.Werewolf);
   
    public int VillagerTeamCount 
        => AssignedRoles.Count(r => r.GetTeam() == Team.Villager);

    public void Receive(SetupRoleChangedMessage message)
    {
        Console.WriteLine("Received message");
        Roles[message.Index] = message.Role;
        
        GrowRolesAsNeeded();
        StateHasChanged();
    }

    private void GrowRolesAsNeeded()
    {
        while (Roles.Count < PlayerCount + 3)
        {
            Roles.Add(GameRole.Villager); // Good default
        }
        Console.WriteLine($"Roles at {Roles.Count} for player count of {PlayerCount}: {RolesDebugString}");
    }

    public bool HumanControlsPlayerOne { get; set; } = true;

    private void SetPlayerCount(int count)
    {
        Console.WriteLine($"Player count set to {count}");
        PlayerCount = count;
        GrowRolesAsNeeded();
        StateHasChanged();
    }

    public bool IsStartingGame { get; set; }
    
    public async Task StartGameClicked()
    {
        IsStartingGame = true;
        StateHasChanged();

        await Task.Run(StartGameAndNavigate);
    }

    private void StartGameAndNavigate()
    {
        // Create the game
        GameSetup setup = new();
        setup.AddRoles(AssignedRoles);
        
        // Set players
        IRoleClaimStrategy roleClaimStrategy = new ClaimSafestRoleStrategy(new Random());
        RandomOptimalVoteController aiController = new RandomOptimalVoteController(roleClaimStrategy);
        
        Player[] players = Enumerable.Range(1, PlayerCount)
            .Select(n =>
            {
                PlayerController controller = n == 1 && HumanControlsPlayerOne
                    ? _playerController
                    : aiController;
                return new Player($"Player {n}", controller);
            })
            .ToArray();
        setup.AddPlayers(players);
        
        // Store the game in the services collection
        Guid gameId = _gameService.StartGame(setup);
        
        // Navigate to the game with the specified Id
        _nav.NavigateTo($"/games/{gameId}");
    }
}