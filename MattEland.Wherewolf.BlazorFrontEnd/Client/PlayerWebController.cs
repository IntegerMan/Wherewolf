using CommunityToolkit.Mvvm.Messaging;
using MattEland.Wherewolf.BlazorFrontEnd.Messages;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.BlazorFrontEnd.Client;

public class PlayerWebController : PlayerController, 
    IRecipient<RoleClaimedMessage>, 
    IRecipient<VotedMessage>, 
    IRecipient<RobbedPlayerMessage>,
    IRecipient<LoneWolfSlotSelectionMessage>,
    IRecipient<SpecificClaimMadeMessage>
{
    private Action<Player>? _voteCallback;
    private Action<Player>? _robberCallback;
    private Action<GameSlot>? _loneWolfCallback;
    private Action<GameRole>? _initialRoleClaimCallback;
    private Action<SpecificRoleClaim>? _specificRoleClaimCallback;

    public PlayerWebController()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    
    public override void SelectLoneWolfCenterCard(GameSlot[] centerSlots, Action<GameSlot> callback)
    {
        _loneWolfCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.SelectLoneWolfPeekCard));
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState gameState, Player robbingPlayer, Action<Player> callback)
    {
        _robberCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.SelectRobberVictim));
    }

    public override void GetPlayerVote(Player votingPlayer, GameState state, PlayerProbabilities playerProbs, Dictionary<Player, double> victoryProbs, Action<Player> callback)
    {
        _voteCallback = callback;
        WeakReferenceMessenger.Default.Send(new VoteRequestedMessage(state, playerProbs, victoryProbs));
    }

    public override void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback)
    {
        _initialRoleClaimCallback = callback;
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.InitialRoleClaim));
    }

    public override void GetSpecificRoleClaim(Player player, GameState gameState, GameRole initialRoleClaim,
        SpecificRoleClaim[] possibleClaims, Action<SpecificRoleClaim> callback)
    {
        _specificRoleClaimCallback = callback;
        WeakReferenceMessenger.Default.Send(new SpecificRoleClaimNeededMessage(possibleClaims, initialRoleClaim));
    }

    public void Receive(RoleClaimedMessage message)
    {
        Action<GameRole>? callback = _initialRoleClaimCallback;
        if (callback is null) return;
        
        _initialRoleClaimCallback = null;
        
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

    public void Receive(RobbedPlayerMessage message)
    {
        Action<Player>? callback = _robberCallback;
        if (callback is null) return;
        
        _robberCallback = null;
        
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.Normal));
        callback(message.Target);
    }

    public void Receive(LoneWolfSlotSelectionMessage message)
    {
        Action<GameSlot>? callback = _loneWolfCallback;
        if (callback is null) return;
        
        _loneWolfCallback = null;
        
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.Normal));
        callback(message.Slot);
    }

    public void Receive(SpecificClaimMadeMessage message)
    {
        Action<SpecificRoleClaim>? callback = _specificRoleClaimCallback;
        if (callback is null) return;
        
        _specificRoleClaimCallback = null;
        
        WeakReferenceMessenger.Default.Send(new ChangeClientModeMessage(ClientMode.Normal));
        callback(message.Claim);
    }
}