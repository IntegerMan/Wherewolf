using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public interface IGameEvent
{
    string Description { get; }
    Team? AssociatedTeam { get; }
}