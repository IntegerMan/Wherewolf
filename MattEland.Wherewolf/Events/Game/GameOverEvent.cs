using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class GameOverEvent : GameEvent
{
    internal GameOverEvent(Team winningTeam)
    {
        AssociatedTeam = winningTeam;
    }

    public override bool IsObservedBy(Player player) => true;
    public override string Description => AssociatedTeam == Team.Villager ?
        "The villagers have won!" :
        "The werewolves have won!";
    public override Team? AssociatedTeam { get; }
}