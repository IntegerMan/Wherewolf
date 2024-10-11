using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public abstract class PlayerController
{
    public abstract string SelectLoneWolfCenterCard(string[] centerSlotNames);

    public abstract string SelectRobberTarget(string[] otherPlayerNames);

    public virtual void ObservedEvent(GameEvent gameEvent, GameState state)
    {
    }

    public abstract Player GetPlayerVote(Player votingPlayer, GameState gameState);

    public abstract GameRole GetInitialRoleClaim(GameState gameState);
}