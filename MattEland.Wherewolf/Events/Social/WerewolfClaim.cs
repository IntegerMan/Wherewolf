using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

/// <summary>
/// Represents a player explicitly claiming to have been a werewolf and woken with at least one other werewolf
/// </summary>
/// <remarks>
/// At the moment we'll only be supporting a max of two wolves per game, but this should be flexible for larger WW teams and more WW roles
/// </remarks>
public class WokeWithWerewolvesClaim(Player player, params Player[] otherWerewolves) : SpecificRoleClaim(player, GameRole.Werewolf)
{
    public Player[] OtherWerewolves => otherWerewolves;

    public override string Description => $"{Player} claims to have been a werewolf and woke with {string.Join("and ", OtherWerewolves.Select(w => w.Name))}";
    
    public override bool IsClaimValidFor(GameState state) 
        => state.Events.OfType<SawOtherWolvesEvent>()
            .Any(e => e.Players.Contains(Player.Name) && OtherWerewolves.All(w => e.Players.Contains(w.Name)) && e.Players.Count == OtherWerewolves.Length + 1);
}