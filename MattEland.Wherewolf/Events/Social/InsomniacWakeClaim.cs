using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

/// <summary>
/// Represents a claim made by a player that they were an insomniac and saw a specific role when they woke
/// </summary>
/// <param name="player">The player who claims to have started as an insomniac</param>
/// <param name="finalRole">The final role they observed</param>
public class InsomniacWakeClaim(Player player, GameRole finalRole) : SpecificRoleClaim(player, GameRole.Insomniac)
{
    public override string Description => $"{Player} claimed they were an insomniac and saw {finalRole} when they woke";
    public GameRole FinalRole => finalRole;

    public override bool IsClaimValidFor(GameState state) 
        => state.Events.OfType<InsomniacSawFinalCardEvent>()
            .Any(e => e.Player == Player && e.Role == FinalRole);
}