using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class EventTimeline : ComponentBase
{
    [Parameter] public GameState? Game { get; set; }
    [Parameter] public Player? Perspective { get; set; }
    
    public IEnumerable<IGameEvent> VisibleEvents 
        => Game switch
        {
            null => [],
            _ => Game.EventsForPlayer(Perspective)
        };
    
        
    private Color CalculateEventColor(IGameEvent evt)
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