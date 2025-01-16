using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class ConfigureRoleSlot
{
    private static ConfigureRoleSlot Instance { get; set; } = null!;

    [Parameter] public GameRole? Role { get; set; }
    [Parameter] public int Index { get; set; }

    private void HandleDroppedInstance(string role)
    {
        Role = Enum.Parse<GameRole>(role);
        
        WeakReferenceMessenger.Default.Send(new SetupRoleChangedMessage(Index, Role));

        StateHasChanged();
    }

    [JSInvokable]
    public static void HandleDropped(string role)
    {
        Instance.HandleDroppedInstance(role);
    }

    private void SetTarget() => Instance = this;
}