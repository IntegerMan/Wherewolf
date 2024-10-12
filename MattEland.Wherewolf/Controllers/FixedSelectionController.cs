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
    
    public override string SelectLoneWolfCenterCard(string[] centerSlotNames)
    {
        string next = _selection.Dequeue();
        return centerSlotNames.Single(s => s == next);
    }

    public override string SelectRobberTarget(string[] otherPlayerNames)
    {
        string next = _selection.Dequeue();
        return otherPlayerNames.Single(s => s == next);
    }

    public override Player GetPlayerVote(Player votingPlayer, GameState gameState)
    {
        string next = _selection.Dequeue();
        return gameState.Players.Single(p => p.Name == next);
    }

    public override GameRole GetInitialRoleClaim(Player player, GameState gameState) => 
        _roleClaimStrategy.GetRoleClaim(player, gameState);
}