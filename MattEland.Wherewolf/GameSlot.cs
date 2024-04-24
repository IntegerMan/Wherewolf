using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameSlot
{
    public GameSlot(string name, GameRole startRole, Player? player = null)
    {
        this.Name = name;
        this.Player = player;
        this.StartRole = startRole;
        this.CurrentRole = startRole;
    }
    
    public GameSlot(GameSlot baseSlot)
    {
        this.Name = baseSlot.Name;
        this.Player = baseSlot.Player;
        this.StartRole = baseSlot.StartRole;
        this.CurrentRole = baseSlot.CurrentRole;
    }
    
    public GameSlot(GameSlot baseSlot, GameRole newRole)
    {
        this.Name = baseSlot.Name;
        this.Player = baseSlot.Player;
        this.StartRole = baseSlot.StartRole;
        this.CurrentRole = newRole;
    }    

    public string Name { get; init; }
    public Player? Player { get; init; }
    public GameRole StartRole { get; init; }
    public GameRole CurrentRole { get; init; }
}