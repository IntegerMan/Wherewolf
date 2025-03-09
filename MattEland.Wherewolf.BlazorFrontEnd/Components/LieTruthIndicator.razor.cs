using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class LieTruthIndicator : ComponentBase
{
    [Parameter] public bool? IsTruth { get; set; }
}