using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class PlayerIndicator : ComponentBase
{
    [Parameter]
    public required Player Player { get; set; }
}