using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameSlot
{
    public GameSlot(string name, GameRole startRole, Player? player = null)
    {
        Name = name;
        Player = player;
        Role = startRole;
    }
    
    public GameSlot(GameSlot baseSlot)
    {
        Name = baseSlot.Name;
        Player = baseSlot.Player;
        Role = baseSlot.Role;
    }

    public string Name { get; }
    public Player? Player { get; }
    public GameRole Role { get; }
}