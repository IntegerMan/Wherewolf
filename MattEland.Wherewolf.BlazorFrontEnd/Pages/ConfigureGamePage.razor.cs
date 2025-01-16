using MattEland.Wherewolf.Roles;
using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class ConfigureGamePage : IRecipient<SetupRoleChangedMessage>
{
    public int PlayerCount { get; set; } = 5;

    public ConfigureGamePage()
    {
        PlayerCount = 3;
        Roles = [..Enumerable.Repeat<GameRole?>(null, PlayerCount + 3)];
        
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
            Console.WriteLine($"Checking roles: {allRolesAssigned}: {string.Join(", ", AssignedRoles.Select(r => r.HasValue ? r.Value.ToString() : "Unassigned"))}");
            return allRolesAssigned;
        }
    }

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
        while (PlayerCount + 3 < Roles.Count)
        {
            Roles.Add(null);
        }
    }

    public bool HumanControlsPlayerOne { get; set; } = true;

    private void SetPlayerCount(int count)
    {
        PlayerCount = count;
        GrowRolesAsNeeded();
        StateHasChanged();
    }
}