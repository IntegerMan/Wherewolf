using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.BlazorFrontEnd.Repositories;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;
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
    
    public IEnumerable<GameEvent> VisibleEvents 
        => Game switch
        {
            null => [],
            _ => PerspectivePlayer is null
                ? Game.Events
                : Game.Events.Where(e => e.IsObservedBy(PerspectivePlayer))
        };

    private Color CalculateEventColor(GameEvent evt) 
        => evt.AssociatedTeam.GetTeamColor();

    private void AdvanceToNextPhase() => Game = Game?.RunNext();
    private void AdvanceToVoting() => Game = Game?.RunToEndOfNight();
    private void AdvanceToEnd() => Game = Game?.RunToEnd();
}