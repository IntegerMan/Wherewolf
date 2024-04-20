namespace MattEland.Wherewolf.Events;

/// <summary>
/// This event occurs during the night Werewolf phase when 2 or more players started as a werewolf
/// </summary>
public class SawOtherWolvesEvent : GameEvent
{
    public IEnumerable<Player> Players { get; }

    public SawOtherWolvesEvent(IEnumerable<Player> wolves)
    {
        this.Players = wolves;
    }
    
    public override bool IsObservedBy(Player player)
    {
        return Players.Contains(player);
    }

    public override string Description
        => $"{string.Join(" and ", Players.Select(p => p.Name))} saw that each other were on the werewolf team";
}