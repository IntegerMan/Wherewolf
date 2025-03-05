using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class GamePage : ComponentBase
{
    [Parameter]
    public Guid gameId { get; set; }
}