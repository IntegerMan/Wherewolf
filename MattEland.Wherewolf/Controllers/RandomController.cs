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

    public override string SelectLoneWolfCenterCard(string[] centerSlotNames) 
        => centerSlotNames.ElementAt(_rand.Next(centerSlotNames.Length));

    public override Player SelectRobberTarget(Player[] otherPlayers, GameState state, Player robber)
        => otherPlayers.ElementAt(_rand.Next(otherPlayers.Length));

    public override Player GetPlayerVote(Player votingPlayer, GameState state)
        => state.Players.Where(p => p != votingPlayer).ElementAt(_rand.Next(state.Players.Count() - 1));

    public override GameRole GetInitialRoleClaim(Player player, GameState gameState)
        => _roleClaimStrategy.GetRoleClaim(player, gameState);
}