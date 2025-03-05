using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.BlazorFrontEnd.Repositories;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Pages;

public partial class GamePage(IGameStateRepository repo) : ComponentBase
{
    [Parameter]
    public Guid GameId { get; set; }
    
    public GameState? Game { get; set; }
    
    public Player? PerspectivePlayer { get; set; }

    public bool CanAdvance => Game is { IsGameOver: false };
    public bool IsGameOver => Game is { IsGameOver: true };
    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Game = repo.FindGame(GameId);
    }
    
    public IEnumerable<IGameEvent> VisibleEvents 
        => Game switch
        {
            null => [],
            _ => Game.EventsForPlayer(PerspectivePlayer)
        };

    public string NewGameUrl => "/setup";
    
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

    private void AdvanceToNextPhase() => Game = Game?.RunNext();
    private void AdvanceToVoting() => Game = Game?.RunToEndOfNight();
    private void AdvanceToEnd() => Game = Game?.RunToEnd();
}