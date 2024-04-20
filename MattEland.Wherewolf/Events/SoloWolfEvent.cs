namespace MattEland.Wherewolf.Events;

/// <summary>
/// An event that occurs when a werewolf player wakes during the night phase and discovers they
/// are the only werewolf player.
/// </summary>
public class SoloWolfEvent : GameEvent
{
    public Player Player { get; }

    public SoloWolfEvent(Player player)
    {
        this.Player = player;
    }
    
    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description 
        => $"{Player.Name} saw that they were the only werewolf";
}