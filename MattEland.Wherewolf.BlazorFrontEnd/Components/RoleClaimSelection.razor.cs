using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class RoleClaimSelection : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    private void RoleClaimed(GameRole role)
    {
        WeakReferenceMessenger.Default.Send(new RoleClaimedMessage(role));
    }
}