using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public class StartRoleClaimedEvent(Player player, GameRole role, GameState stateAtTime) : SocialEvent(player)
{
    private readonly GameEvent _underlyingEvent = EventPool.DealtCardEvent(player.Name, role);

    public GameRole ClaimedRole => role;

    public override string Description => $"{Player.Name} claimed their starting role was {ClaimedRole}";

    public override bool IsClaimValidFor(GameState gameState) 
        => gameState.Events.Contains(_underlyingEvent);

    public override Team? AssociatedTeam => ClaimedRole.GetTeam();
    public GameState GameState { get; } = stateAtTime;

    public bool CanBeBelievedBy(GameState state, PlayerProbabilities probabilities)
    {
        SlotRoleProbabilities slotProbabilities = probabilities.GetStartProbabilities(state[Player.Name]);
        return slotProbabilities[role].Probability > 0;
    }
    
    public bool CannotBeFalse(GameState state, PlayerProbabilities probabilities)
    {
        SlotRoleProbabilities slotProbabilities = probabilities.GetStartProbabilities(state[Player.Name]);
        return slotProbabilities[role].Probability >= 1;
    }
}