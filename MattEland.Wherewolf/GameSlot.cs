using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public record GameSlot(string Name, GameRole StartRole)
{
    public Player? Player { get; init; }
    public GameRole CurrentRole => StartRole;
}