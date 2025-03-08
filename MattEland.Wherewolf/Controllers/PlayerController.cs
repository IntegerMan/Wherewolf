using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Controllers;

public abstract class PlayerController
{
    public abstract void SelectLoneWolfCenterCard(GameSlot[] centerSlots, Action<GameSlot> callback);

    public abstract void SelectRobberTarget(Player[] otherPlayers, GameState gameState, Player robbingPlayer, Action<Player> callback);

    public virtual void ObservedEvent(GameEvent gameEvent, GameState state)
    {
    }

    public abstract void GetPlayerVote(Player votingPlayer, GameState state, PlayerProbabilities probabilities, Dictionary<Player, double> voteProbabilities, Action<Player> callback);

    public abstract void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback);

    public abstract void GetSpecificRoleClaim(Player player, GameState gameState, GameRole initialRoleClaim, SpecificRoleClaim[] possibleClaims, Action<SpecificRoleClaim> callback);
}