using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

public class StartRoleClaimedEvent(Player player, GameRole role, GameState stateAtTime) : SocialEvent(player)
{
    public GameRole ClaimedRole => role;

    public override string Description => $"{Player.Name} claimed their starting role was {ClaimedRole}";


    public override bool IsClaimValidFor(GameState gameState) 
        => gameState.GetStartRole(Player) == ClaimedRole;

    public override Team? AssociatedTeam => ClaimedRole.GetTeam();
    public GameState GameState => stateAtTime;

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