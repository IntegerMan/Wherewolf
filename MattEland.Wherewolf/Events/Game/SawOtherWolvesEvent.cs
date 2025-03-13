using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// This event occurs during the night Werewolf phase when 2 or more players started as a werewolf
/// </summary>
public class SawOtherWolvesEvent(IEnumerable<Player> wolves) : GameEvent
{
    public IEnumerable<Player> Players { get; } = wolves;

    public override bool IsObservedBy(Player player)
    {
        return Players.Contains(player);
    }
    
    public override Team? AssociatedTeam => Team.Werewolf;

    public override string Description
        => $"{string.Join(" and ", Players.Select(p => p.Name))} saw that each other were on the werewolf team";
    
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
            if (state.GetStartRole(slot).GetTeam() == Team.Werewolf && !Players.Contains(slot.Player))
            {
                return false;
            }
        }

        return true;
    }

}