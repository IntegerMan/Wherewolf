using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public struct GameSlot
{
    public string Name { get; set; }
    public Player? Player { get; init; }
    public GameRole StartRole { get; init; }
    public GameRole CurrentRole { get; set; }
}