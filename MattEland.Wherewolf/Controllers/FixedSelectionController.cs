using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class FixedSelectionController : PlayerController
{
    private readonly Queue<string> _selection;
    private readonly IRoleClaimStrategy _roleClaimStrategy;

    public FixedSelectionController(IRoleClaimStrategy roleClaimStrategy, params string[] selection)
    {
        _roleClaimStrategy = roleClaimStrategy;
        _selection = new Queue<string>(selection);
    }
    
    public override void SelectLoneWolfCenterCard(string[] centerSlotNames, Action<string> callback)
    {
        string next = _selection.Dequeue();
        string choice = centerSlotNames.Single(s => s == next);
        callback(choice);
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState state, Player robber, Action<Player> callback)
    {
        string next = _selection.Dequeue();
        Player player = otherPlayers.Single(s => s.Name == next);
        callback(player);
    }

    public override void GetPlayerVote(Player votingPlayer, GameState state, Action<Player> callback)
    {
        string next = _selection.Dequeue();
        Player choice = state.Players.Single(p => p.Name == next);
        callback(choice);
    }

    public override void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback)
    {
        GameRole choice = _roleClaimStrategy.GetRoleClaim(player, gameState);
        callback(choice);
    }
}