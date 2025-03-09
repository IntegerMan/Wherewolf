using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class RoleIndicator : ComponentBase
{
    [Parameter]
    public required GameRole Role { get; set; }
    
    [Parameter]
    public bool CenterAlign { get; set; }
}