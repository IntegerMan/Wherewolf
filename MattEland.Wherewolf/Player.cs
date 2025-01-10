using MattEland.Wherewolf.Controllers;

namespace MattEland.Wherewolf;

public record Player(string Name, PlayerController Controller)
{
    public int Order { get; set; }

    public override string ToString() => Name;

    public override int GetHashCode() => Name.GetHashCode();
}