using System.Collections.Immutable;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameResult(IEnumerable<Player> dead, GameState state)
{
    public IEnumerable<Player> DeadPlayers { get; } = dead.ToImmutableList();
    public IEnumerable<GameRole> DeadRoles => DeadPlayers.Select(p => state[p.Name].Role).Distinct();
}