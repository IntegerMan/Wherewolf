using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// This event occurs during the night Werewolf phase when 2 or more players started as a werewolf
/// </summary>
public class SawOtherWolvesEvent(IEnumerable<string> wolves) : GameEvent
{
    public HashSet<string> Players { get; } = wolves.ToHashSet();

    public override bool IsObservedBy(Player player) => Players.Contains(player.Name);

    public override Team? AssociatedTeam => Team.Werewolf;

    public override string Description => $"{string.Join(" and ", Players)} saw that each other were on the werewolf team";
    
    public override bool IsPossibleInGameState(GameState state)
    {
        if (state.ContainsEvent(this)) return true;
        
        // Can only occur if all listed players are werewolves
        foreach (var player in Players)
        {
            if (state.GetStartRole(player).GetTeam() != Team.Werewolf)
            {
                return false;
            }
        }
        
        // Cannot occur if other players not listed are also werewolves
        foreach (var slot in state.PlayerSlots)
        {
            if (state.GetStartRole(slot).GetTeam() == Team.Werewolf && !Players.Contains(slot.Player!.Name))
            {
                return false;
            }
        }

        return true;
    }

}