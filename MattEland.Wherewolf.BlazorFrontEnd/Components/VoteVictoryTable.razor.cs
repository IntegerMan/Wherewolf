using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Probability;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class VoteVictoryTable : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required Player PerspectivePlayer { get; set; }
    
    [Parameter]
    public required PlayerProbabilities Probabilities { get; set; }

    [Parameter]
    public Player? VotedPlayer { get; set; }
    
    [Parameter] public bool AllowVoting { get; set; } = true;
    
    public Dictionary<Player, double> Stats { get; private set; } = [];
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Stats = VotingHelper.GetVoteVictoryProbabilities(PerspectivePlayer, Game);
    }
    
    private void PlayerSelected(Player player)
    {
        VotedPlayer = player;
        AllowVoting = false;
        WeakReferenceMessenger.Default.Send(new VotedMessage(PerspectivePlayer, player));
    }
}