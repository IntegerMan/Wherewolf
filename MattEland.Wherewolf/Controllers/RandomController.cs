using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class RandomController : PlayerController
{
    private readonly Random _rand;
    private readonly IRoleClaimStrategy _roleClaimStrategy;

    protected Random Rand => _rand;

    public RandomController(IRoleClaimStrategy? roleClaimStrategy = null, Random? rand = null)
    {
        _roleClaimStrategy = roleClaimStrategy ?? new ClaimStartingRoleStrategy();
        _rand = rand ?? new Random();
    }

    public override void SelectLoneWolfCenterCard(GameSlot[] centerSlots, Action<GameSlot> callback)
    {
        GameSlot choice = centerSlots.ElementAt(_rand.Next(centerSlots.Length));
        callback(choice);
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState state, Player robber, Action<Player> callback)
    {
        Player choice = otherPlayers.ElementAt(_rand.Next(otherPlayers.Length));
        callback(choice);
    }

    public override void GetPlayerVote(Player votingPlayer, GameState state, Action<Player> callback)
    {
        Player choice = state.Players.Where(p => p != votingPlayer)
            .ElementAt(_rand.Next(state.Players.Count() - 1));
        callback(choice);
    }

    public override void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback)
    {
        GameRole choice = _roleClaimStrategy.GetRoleClaim(player, gameState);
        callback(choice);
    }
}