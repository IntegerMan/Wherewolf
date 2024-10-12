using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public class RandomController : PlayerController
{
    private readonly Random _rand;
    private readonly IRoleClaimStrategy _roleClaimStrategy;

    public RandomController(IRoleClaimStrategy? roleClaimStrategy = null, Random? rand = null)
    {
        _roleClaimStrategy = roleClaimStrategy ?? new ClaimStartingRoleStrategy();
        _rand = rand ?? new Random();
    }

    public override string SelectLoneWolfCenterCard(string[] centerSlotNames) 
        => centerSlotNames.ElementAt(_rand.Next(centerSlotNames.Length));

    public override string SelectRobberTarget(string[] otherPlayerNames)
        => otherPlayerNames.ElementAt(_rand.Next(otherPlayerNames.Length));

    public override Player GetPlayerVote(Player votingPlayer, GameState gameState)
        => gameState.Players.Where(p => p != votingPlayer).ElementAt(_rand.Next(gameState.Players.Count() - 1));

    public override GameRole GetInitialRoleClaim(Player player, GameState gameState)
        => _roleClaimStrategy.GetRoleClaim(player, gameState);
}