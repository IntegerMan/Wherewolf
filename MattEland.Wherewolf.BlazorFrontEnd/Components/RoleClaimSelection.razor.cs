using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Microsoft.AspNetCore.Components;

namespace MattEland.Wherewolf.BlazorFrontEnd.Components;

public partial class RoleClaimSelection : ComponentBase
{
    [Parameter]
    public required GameState Game { get; set; }
    
    [Parameter]
    public required Player PerspectivePlayer { get; set; }

    public IEnumerable<KeyValuePair<GameRole, VoteStatistics>> Stats { get; private set; } = [];
    
    private void RoleClaimed(GameRole role)
    {
        WeakReferenceMessenger.Default.Send(new RoleClaimedMessage(role));
    }
    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Stats = VotingHelper.GetRoleClaimVoteStatistics(PerspectivePlayer, Game);
    }
}