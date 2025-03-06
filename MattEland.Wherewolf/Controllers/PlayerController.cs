using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public abstract class PlayerController
{
    public abstract void SelectLoneWolfCenterCard(string[] centerSlotNames, Action<string> callback);

    public abstract void SelectRobberTarget(Player[] otherPlayers, GameState gameState, Player robbingPlayer, Action<Player> callback);

    public virtual void ObservedEvent(GameEvent gameEvent, GameState state)
    {
    }

    public abstract void GetPlayerVote(Player votingPlayer, GameState state, Action<Player> callback);

    public abstract void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback);
}