using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

/// <summary>
/// An event that occurs when a card is dealt to a slot.
/// </summary>
public class DealtCardEvent : GameEvent
{
    public GameRole Card { get; }
    public GameSlot Slot { get; }
    public Player? Player { get; }

    public DealtCardEvent(GameRole card, GameSlot slot)
    {
        this.Card = card;
        this.Slot = slot;
        this.Player = slot.Player;
    }

    public override bool IsObservedBy(Player player) => Player == player;
    public override string Description => $"{Slot.Name} was dealt {Card.Name}";
}