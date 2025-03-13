using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class VotedOutEvent : GameEvent
{
    private readonly Player _player;
    private readonly GameRole _finalRole;

    internal VotedOutEvent(Player player, GameRole finalRole)
    {
        _player = player;
        _finalRole = finalRole;
    }

    public override bool IsObservedBy(Player _) => true;

    public override string Description => $"{_player.Name} was voted out as a {_finalRole}";

    public override Team? AssociatedTeam => _finalRole.GetTeam();

    public Player Player => _player;
    public GameRole Role => _finalRole;
}