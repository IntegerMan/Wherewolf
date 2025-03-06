using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.BlazorFrontEnd.Helpers;

public class PlayerWebController : PlayerController
{
    public override void SelectLoneWolfCenterCard(string[] centerSlotNames, Action<string> callback)
    {
        throw new NotImplementedException();
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState gameState, Player robbingPlayer, Action<Player> callback)
    {
        throw new NotImplementedException();
    }

    public override void GetPlayerVote(Player votingPlayer, GameState state, Action<Player> callback)
    {
        throw new NotImplementedException();
    }

    public override void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback)
    {
        throw new NotImplementedException();
    }
}