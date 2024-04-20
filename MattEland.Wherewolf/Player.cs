using MattEland.Wherewolf.Controllers;

namespace MattEland.Wherewolf;

public record Player(string Name, PlayerController Controller)
{
    public int Order { get; set; }
}