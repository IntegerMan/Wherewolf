using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public abstract class PlayerController
{
    public abstract string SelectLoneWolfCenterCard(string[] centerSlotNames);

    public abstract Player SelectRobberTarget(Player[] otherPlayers, GameState gameState, Player robbingPlayer);

    public virtual void ObservedEvent(GameEvent gameEvent, GameState state)
    {
    }

    public abstract Player GetPlayerVote(Player votingPlayer, GameState state);

    public abstract GameRole GetInitialRoleClaim(Player player, GameState gameState);

    public virtual void RunningPhase(GamePhase phase, GameState gameState)
    {
    }

    public virtual void RanPhase(GamePhase phase, GameState gameState)
    {
    }
}