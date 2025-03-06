using System.Text;
using ApexCharts;
using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
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
    public PlayerProbabilities PlayerProbabilities { get; set; }

    public string CardClass
    {
        get
        {
            KeyValuePair<GameRole,SlotProbability>[] possible = Probabilities.Where(p => p.Value.Probability > 0)
                    .ToArray();

            if (possible.Length == 1 || possible.GroupBy(p => p.Key.GetTeam()).Count() == 1)
            {
                return possible.First().Key.GetTeamTheme();
            }
            
            return "mud-theme-dark";
        }
    }

    public string CardIcon => GetRoleIcon(PresumedGameRole);

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

    private static string GetRoleIcon(GameRole? role) => role switch
    {
        null => Icons.Material.Filled.QuestionMark,
        GameRole.Villager => Icons.Material.Filled.Person,
        GameRole.Werewolf => Icons.Material.Filled.Bedtime,
        GameRole.Robber => Icons.Material.Filled.AttachMoney,
        GameRole.Insomniac => Icons.Material.Filled.Coffee,
        _ => Icons.Material.Filled.Error
    };

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
            
            StringBuilder sb = new();
            foreach (var prob in Probabilities
                         .Where(p => p.Value.Probability > 0)
                         .OrderByDescending(p => p.Value.Probability))
            {
                sb.Append($"{prob.Key} ({prob.Value.Probability:P0}) ");
            }
            return sb.ToString().TrimEnd();
        }
    }
}

public class ChartDataItem
{
    public required string Name { get; init; }
    public decimal Value { get; init; }
}