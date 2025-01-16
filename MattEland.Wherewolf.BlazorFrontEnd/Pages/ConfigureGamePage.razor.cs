using MattEland.Wherewolf.Roles;
using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class ConfigureGamePage : IRecipient<SetupRoleChangedMessage>
{
    public int PlayerCount { get; set; }

    public ConfigureGamePage()
    {
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
}