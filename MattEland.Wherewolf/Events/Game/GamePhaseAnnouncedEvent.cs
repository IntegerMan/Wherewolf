using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class GamePhaseAnnouncedEvent : GameEvent
{
    internal GamePhaseAnnouncedEvent(string message, GameRole? associatedRole = null)
    {
        Description = message;
        AssociatedRole = associatedRole;
    }

    public override bool IsObservedBy(Player player) => true;
    
    public GameRole? AssociatedRole { get; }

    public override string Description { get; }

    public override bool IsPossibleInGameState(GameState state) => true;
}