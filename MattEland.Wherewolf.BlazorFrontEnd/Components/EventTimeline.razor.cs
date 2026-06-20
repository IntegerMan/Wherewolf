using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class EventTimeline : ComponentBase
{
    [Parameter] public required GameManager Game { get; set; }
    [Parameter] public Player? Perspective { get; set; }

    public IEnumerable<IGameEvent> VisibleEvents 
        => Game switch
        {
            null => [],
            _ => Game.EventsForPlayer(Perspective)
        };

    [Parameter]
    public PlayerProbabilities? Probabilities { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    private ElementReference _scrollAnchor;
    private int _lastEventCount;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var count = VisibleEvents.Count();
        if (count != _lastEventCount)
        {
            _lastEventCount = count;
            await JSRuntime.InvokeVoidAsync("dragDropInterop.scrollIntoView", _scrollAnchor);
        }
    }


    private static Color CalculateEventColor(IGameEvent evt)
    {
        if (evt is GamePhaseAnnouncedEvent)
        {
            return Color.Dark;
        }

        if (evt is VotedEvent or SocialEvent)
        {
            return Color.Primary;
        }
        
        return evt.AssociatedTeam.GetTeamColor();
    }
}