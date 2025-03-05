using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class SlotCardDisplay : ComponentBase
{
    [Parameter]
    public GameSlot Slot { get; set; }
    
    [Parameter]
    public bool IsPlayer { get; set; }
    
    [Parameter]
    public Player? PerspectivePlayer { get; set; }

    public bool IsPerspectivePlayer => IsPlayer && Slot.Player == PerspectivePlayer;

    public string CardClass => IsPerspectivePlayer
        ? "mud-theme-primary"
        : "mud-theme-dark";

    public string? CardIcon => Icons.Material.Filled.QuestionMark;
}