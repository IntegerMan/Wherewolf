using System.Text;
using ApexCharts;
using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class SlotCardDisplay : ComponentBase
{
    [Parameter]
    public required GameSlot Slot { get; set; }
    
    [Parameter]
    public bool IsPlayer { get; set; }
    
    [Parameter]
    public Player? PerspectivePlayer { get; set; }
    
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required PlayerProbabilities PlayerProbabilities { get; set; }

    public string CardClass
    {
        get
        {
            if (IsVotedOut) return "mud-theme-dark";
            
            KeyValuePair<GameRole,SlotProbability>[] possible = Probabilities.Where(p => p.Value.Probability > 0)
                    .ToArray();

            if (possible.Length == 1 || possible.GroupBy(p => p.Key.GetTeam()).Count() == 1)
            {
                return possible.First().Key.GetTeamTheme();
            }
            
            return "mud-theme-dark";
        }
    }

    public string CardIcon => PresumedGameRole.GetRoleIcon();

    private GameRole? PresumedGameRole
    {
        get
        {
            var validProbs = Probabilities
                .Where(p => p.Value.Probability > 0)
                .ToArray();

            GameRole? role = null;
            if (validProbs.Length == 1)
            {
                role = validProbs.First().Key;
            }

            return role;
        }
    }

    public ApexChartOptions<ChartDataItem> ChartOptions { get; set; } = new()
    {
        //Title = new Title() {Text ="Role Probabilities"},
        Legend = new Legend()
        {
            Show = false,
        }
    };

    public IEnumerable<ChartDataItem> ChartDataItems => Probabilities
        .OrderBy(p => p.Key.ToString()).Select(p => new ChartDataItem()
        {
            Name = p.Key.ToString(),
            Value = (decimal)p.Value.Probability * 100.0m
        });
    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // At end of game we have complete information visible
        if (Game.IsGameOver)
        {
            Probabilities = new SlotRoleProbabilities();
            Probabilities.SetProbability(Slot.Role, 1, 1, []);
        }
        else
        {
            Probabilities = PlayerProbabilities.GetCurrentProbabilities(Slot);
        }
    }

    public SlotRoleProbabilities Probabilities { get; private set; } = new();
    
    public string Text
    {
        get
        {
            GameRole? role = PresumedGameRole;
            if (role.HasValue)
            {
                return role.Value.ToString();
            }
            
            KeyValuePair<GameRole, SlotProbability> probs = Probabilities
                .Where(p => p.Value.Probability > 0)
                .OrderByDescending(p => p.Value.Probability)
                .First();
            
            return $"{probs.Key}?";
        }
    }

    public int Votes => Game.GameResult == null || Slot.Player == null ? 0 : Game.GameResult.Votes[Slot.Player];
    
    public bool IsVotedOut => Game.GameResult != null && Game.GameResult.DeadPlayers.Contains(Slot.Player);
}

public class ChartDataItem
{
    public required string Name { get; init; }
    public decimal Value { get; init; }
}