using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public class StartRoleClaimedEvent(Player player, GameRole role) : SocialEvent
{
    public Player Player { get; } = player;
    public GameRole ClaimedRole { get;  } = role;

    public override string Description => $"{Player.Name} claimed their starting role was {ClaimedRole}";
    public override bool IsPossibleInGameState(GameState state) =>
        state.Claims
            .OfType<StartRoleClaimedEvent>()
            .Any(e => e.Player == Player && e.ClaimedRole == ClaimedRole);

    public override bool IsClaimValidFor(GameState gameState) 
        => gameState.GetStartRole(Player) == ClaimedRole;

    public override Team? AssociatedTeam => ClaimedRole.GetTeam();

    public bool CanBeBelievedBy(GameState state, PlayerProbabilities probabilities)
    {
        SlotRoleProbabilities slotProbabilities = probabilities.GetStartProbabilities(state[Player.Name]);
        return slotProbabilities[role].Probability > 0;
    }
}