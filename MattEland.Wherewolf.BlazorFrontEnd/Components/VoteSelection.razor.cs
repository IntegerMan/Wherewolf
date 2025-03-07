using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class VoteSelection : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required Player PerspectivePlayer { get; set; }
    
    [Parameter]
    public required PlayerProbabilities Probabilities { get; set; }
    
    private void PlayerSelected(Player player)
    {
        WeakReferenceMessenger.Default.Send(new VotedMessage(PerspectivePlayer, player));
    }
    
    public Dictionary<Player, double> Stats { get; private set; } = [];
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Stats = VotingHelper.GetVoteVictoryProbabilities(PerspectivePlayer, Game);
    }
}