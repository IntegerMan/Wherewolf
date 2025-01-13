using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// An event that occurs when a card is dealt to a slot.
/// </summary>
public class DealtCardEvent(GameRole role, GameSlot slot) : GameEvent
{
    public GameRole Role { get; } = role;
    public GameSlot Slot { get; } = slot;
    public Player? Player { get; } = slot.Player;

    public override bool IsObservedBy(Player player) => Player == player;
    public override string Description => $"{Slot.Name} was dealt {Role}";
    
    public override bool IsPossibleInGameState(GameState state)
    {
        GameSlot stateSlot = state.Root[Slot.Name];
        
        return stateSlot.Role == this.Role;
    }
}