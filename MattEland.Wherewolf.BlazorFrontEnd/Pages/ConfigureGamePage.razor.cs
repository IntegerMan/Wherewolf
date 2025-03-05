using MattEland.Wherewolf.Roles;
using CommunityToolkit.Mvvm.Messaging;
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

    public int PlayerCount { get; set; }

    public ConfigureGamePage(GameService gameService, NavigationManager nav)
    {
        _gameService = gameService;
        _nav = nav;
        PlayerCount = 5;
        Roles = [GameRole.Werewolf, GameRole.Werewolf, GameRole.Villager, GameRole.Villager, GameRole.Robber, GameRole.Insomniac];
        GrowRolesAsNeeded();
        
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public List<GameRole?> Roles { get; }

    public GameRole?[] AssignedRoles 
        => Roles.Take(PlayerCount + 3).ToArray();

    public string[] TickLabels 
        => Enumerable.Range(3, 10).Select(i => i.ToString()).ToArray();
    
    public bool AllRolesAssigned
    {
        get
        {
            bool allRolesAssigned = AssignedRoles.All(r => r.HasValue);
            Console.WriteLine($"Checking roles: {allRolesAssigned}: {RolesDebugString}");
            return allRolesAssigned;
        }
    }

    public string RolesDebugString 
        => string.Join(", ", AssignedRoles.Select(r => r.HasValue 
            ? r.Value.ToString() 
            : "Unassigned"));

    public bool IsValid => AllRolesAssigned;
    
    public int WerewolfTeamCount
        => AssignedRoles.Count(r => r.HasValue && r.Value.GetTeam() == Team.Werewolf);
   
    public int VillagerTeamCount 
        => AssignedRoles.Count(r => r.HasValue && r.Value.GetTeam() == Team.Villager);

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
        
        // Set roles
        GameRole[] roles = Roles
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToArray();
        setup.AddRoles(roles);
        
        // Set players
        Player[] players = Enumerable.Range(1, PlayerCount)
            .Select(n =>
            {
                PlayerController controller = n == 1 && HumanControlsPlayerOne
                    ? new RandomController() // TODO: Web-based controller needed
                    : new RandomController();
                return new Player("Player " + n, controller);
            })
            .ToArray();
        setup.AddPlayers(players);
        
        // Store the game in the services collection
        Guid gameId = _gameService.StartGame(setup);
        
        // Navigate to the game with the specified Id
        _nav.NavigateTo($"/games/{gameId}");
    }
}