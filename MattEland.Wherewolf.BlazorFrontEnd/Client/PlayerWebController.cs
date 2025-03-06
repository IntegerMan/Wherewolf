using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.BlazorFrontEnd.Client;

public class PlayerWebController : PlayerController, IRecipient<RoleClaimedMessage>, IRecipient<VotedMessage>
{
    private Action<GameRole>? _roleClaimCallback;
    private Action<Player>? _voteCallback;
    private Action<Player>? _robberCallback;
    private Action<string>? _loneWolfCallback;

    public PlayerWebController()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    
    public override void SelectLoneWolfCenterCard(string[] centerSlotNames, Action<string> callback)
    {
        _loneWolfCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.SelectLoneWolfPeekCard));
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState gameState, Player robbingPlayer, Action<Player> callback)
    {
        _robberCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.SelectRobberVictim));
    }

    public override void GetPlayerVote(Player votingPlayer, GameState state, Action<Player> callback)
    {
        _voteCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.Vote));
    }

    public override void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback)
    {
        _roleClaimCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.RoleClaim));
    }

    public void Receive(RoleClaimedMessage message)
    {
        Action<GameRole>? callback = _roleClaimCallback;
        if (callback is null) return;
        
        _roleClaimCallback = null;
        
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.Normal));    
        callback(message.Role);
    }

    public void Receive(VotedMessage message)
    {
        Action<Player>? callback = _voteCallback;
        if (callback is null) return;
        
        _voteCallback = null;
        
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.Normal));
        callback(message.Target);
    }
}