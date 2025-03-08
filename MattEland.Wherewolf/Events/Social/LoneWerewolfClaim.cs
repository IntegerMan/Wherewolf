using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

/// <summary>
/// Represents a player explicitly claiming to have been the only waking werewolf and having looked at a center card
/// as a result
/// </summary>
public class LoneWerewolfClaim(Player player, GameSlot centerSlot, GameRole observedRole) : SpecificRoleClaim(player, GameRole.Werewolf)
{
    public GameSlot CenterSlot => centerSlot;
    public GameRole ObservedRole => observedRole;
    
    public override string Description => $"{Player} claims to have been the only werewolf. They looked at {CenterSlot} and saw {ObservedRole}";
    public override bool IsClaimValidFor(GameState state) =>
        state.GetStartRole(Player) == GameRole.Werewolf
        && state.Events.OfType<LoneWolfLookedAtSlotEvent>()
            .Any(e => e.Player == Player && e.Slot == CenterSlot && e.ObservedRole == ObservedRole);
}