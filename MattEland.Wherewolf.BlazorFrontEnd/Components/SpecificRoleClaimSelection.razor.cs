using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class SpecificRoleClaimSelection : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required Player PerspectivePlayer { get; set; }
    
    [Parameter]
    public required SpecificRoleClaimNeededMessage DataSource { get; set; }

    public SpecificRoleClaim? SelectedClaim { get; set; }

    private void OnClaimSelected()
    {
        if (SelectedClaim is null)
        {
            return;
        }
        WeakReferenceMessenger.Default.Send(new SpecificClaimMadeMessage(SelectedClaim));
    }
}