using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using Microsoft.AspNetCore.Components;
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