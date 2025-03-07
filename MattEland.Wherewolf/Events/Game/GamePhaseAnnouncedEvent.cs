using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class GamePhaseAnnouncedEvent(string message, GameRole? associatedRole) : GameEvent
{
    public override bool IsObservedBy(Player player) => true;
    
    public GameRole? AssociatedRole => associatedRole;

    public override string Description  => message;
    public override bool IsPossibleInGameState(GameState state) => true;
}