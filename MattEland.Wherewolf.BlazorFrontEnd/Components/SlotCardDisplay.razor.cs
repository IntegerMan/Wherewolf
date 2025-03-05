using System.Text;
using MattEland.Wherewolf.BlazorFrontEnd.Helpers;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class SlotCardDisplay : ComponentBase
{
    [Parameter]
    public GameSlot Slot { get; set; }
    
    [Parameter]
    public bool IsPlayer { get; set; }
    
    [Parameter]
    public Player? PerspectivePlayer { get; set; }
    
    [Parameter]
    public GameState Game { get; set; }
    
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

    public string? CardIcon => Icons.Material.Filled.QuestionMark;

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