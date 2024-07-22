namespace MattEland.Wherewolf.Controllers;

public class FixedSelectionController : PlayerController
{
    private readonly Queue<string> _selection;
    
    public FixedSelectionController(params string[] selection)
    {
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
}