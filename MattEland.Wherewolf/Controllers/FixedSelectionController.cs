using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
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
    
    public override void SelectLoneWolfCenterCard(GameSlot[] centerSlots, Action<GameSlot> callback)
    {
        string next = _selection.Dequeue();
        GameSlot choice = centerSlots.Single(s => s.Name == next);
        callback(choice);
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState state, Player robber, Action<Player> callback)
    {
        string next = _selection.Dequeue();
        Player player = otherPlayers.Single(s => s.Name == next);
        callback(player);
    }

    public override void GetPlayerVote(Player votingPlayer, GameState state, PlayerProbabilities probabilities, Dictionary<Player, double> voteProbabilities, Action<Player> callback)
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

    public override void GetSpecificRoleClaim(Player player, GameState gameState, GameRole initialRoleClaim, SpecificRoleClaim[] possibleClaims, Action<SpecificRoleClaim> callback)
    {
        // TODO: Probably want to have a way of selecting one of these for testing
        SpecificRoleClaim choice = possibleClaims.First(c => c.Role == initialRoleClaim);
        callback(choice);
    }
}