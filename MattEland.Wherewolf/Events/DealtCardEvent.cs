using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

/// <summary>
/// An event that occurs when a card is dealt to a slot.
/// </summary>
public class DealtCardEvent : GameEvent
{
    public GameRole Role { get; }
    public GameSlot Slot { get; }
    public Player? Player { get; }

    public DealtCardEvent(GameRole role, GameSlot slot)
    {
        this.Role = role;
        this.Slot = slot;
        this.Player = slot.Player;
    }

    public override bool IsObservedBy(Player player) => Player == player;
    public override string Description => $"{Slot.Name} was dealt {Role}";
    
    public override bool IsPossibleInGameState(GameState state)
    {
        GameSlot stateSlot = state.GetSlot(Slot.Name);
        
        return stateSlot.StartRole == this.Role;
    }
}