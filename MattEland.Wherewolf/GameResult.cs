using System.Collections.Immutable;

namespace MattEland.Wherewolf;

public class GameResult(IEnumerable<Player> dead)
{
    public IEnumerable<Player> DeadPlayers { get; } = dead.ToImmutableList();
}