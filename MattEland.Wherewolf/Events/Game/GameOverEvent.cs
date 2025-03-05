using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class GameOverEvent (GameResult result) : GameEvent
{
    public override bool IsObservedBy(Player player) => true;
    public override string Description => result.WinningTeam == Team.Villager ?
        "The villagers have won!" :
        "The werewolves have won!";
    public override Team? AssociatedTeam => result.WinningTeam;
}