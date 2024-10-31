namespace MattEland.Wherewolf.Events;

public class GamePhaseAnnouncedEvent(string message) : GameEvent
{
    public override bool IsObservedBy(Player player) => true;

    public override string Description { get; } = message;
    public override bool IsPossibleInGameState(GameState state) => true;
}