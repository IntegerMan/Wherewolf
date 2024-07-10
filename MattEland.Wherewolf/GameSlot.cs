using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameSlot
{
    public GameSlot(string name, GameRole startRole, Player? player = null)
    {
        this.Name = name;
        this.Player = player;
        this.Role = startRole;
    }
    
    public GameSlot(GameSlot baseSlot)
    {
        this.Name = baseSlot.Name;
        this.Player = baseSlot.Player;
        this.Role = baseSlot.Role;
    }

    public string Name { get; }
    public Player? Player { get; }
    public GameRole Role { get; }
}