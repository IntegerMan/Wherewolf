using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class LoneWolfSelection : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required Player PerspectivePlayer { get; set; }
    
    private void SlotSelected(GameSlot slot)
    {
        WeakReferenceMessenger.Default.Send(new LoneWolfSlotSelectionMessage(slot));
    }
}