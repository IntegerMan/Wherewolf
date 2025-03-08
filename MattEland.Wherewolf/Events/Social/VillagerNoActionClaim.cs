using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

/// <summary>
/// Represents a claim made by a player that they were a villager and did not take an action
/// </summary>
/// <param name="player">The player making the claim</param>
public class VillagerNoActionClaim(Player player) : SpecificRoleClaim(player, GameRole.Villager)
{
    public override string Description => $"{Player} claimed they were a villager and did not take an action";

    public override bool IsClaimValidFor(GameState state) => state.GetStartRole(Player) == GameRole.Villager;
}