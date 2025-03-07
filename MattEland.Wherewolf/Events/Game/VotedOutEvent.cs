using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class VotedOutEvent(Player player, GameRole finalRole) : GameEvent
{
    public override bool IsObservedBy(Player _) => true;

    public override string Description => $"{player.Name} was voted out as a {finalRole}";

    public override Team? AssociatedTeam => finalRole.GetTeam();

    public Player Player => player;
    public GameRole Role => finalRole;
}