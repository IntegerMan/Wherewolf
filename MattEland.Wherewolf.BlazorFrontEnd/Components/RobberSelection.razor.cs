using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class RobberSelection : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required Player PerspectivePlayer { get; set; }
    
    private void PlayerSelected(Player player)
    {
        WeakReferenceMessenger.Default.Send(new RobbedPlayerMessage(player));
    }
}