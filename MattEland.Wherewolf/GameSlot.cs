using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameSlot(string name, GameRole startRole, Player? player = null)
{
    public GameSlot(GameSlot baseSlot) : this(baseSlot.Name, baseSlot.Role, baseSlot.Player)
    {
    }

    public string Name => name;
    public Player? Player => player;
    public GameRole Role => startRole;

    public override string ToString() => Name;
}