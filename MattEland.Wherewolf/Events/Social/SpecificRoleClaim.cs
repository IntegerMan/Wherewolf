using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

/// <summary>
/// Represents a specific role claim made by a player. For example, a player claiming they were an insomniac who saw
/// they were still an insomniac, or a player claiming to be the robber who stole the villager from player X.
/// </summary>
public abstract class SpecificRoleClaim(Player player, GameRole role) : SocialEvent(player)
{
    public GameRole Role => role;
    public override Team? AssociatedTeam => Role.GetTeam();
}