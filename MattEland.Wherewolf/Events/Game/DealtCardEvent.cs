using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// An event that occurs when a card is dealt to a slot.
/// </summary>
public class DealtCardEvent : GameEvent
{
    /// <summary>
    /// An event that occurs when a card is dealt to a slot.
    /// </summary>
    internal DealtCardEvent(string slotName, GameRole role)
    {
        Role = role;
        SlotName = slotName;
    }

    public GameRole Role { get; }
    public string SlotName { get; }

    public override bool IsObservedBy(Player player) => SlotName == player.Name;
    public override string Description => $"{SlotName} was dealt {Role}";
    
    public override bool IsPossibleInGameState(GameState state) => state.ContainsEvent(this);

    public override Team? AssociatedTeam => Role.GetTeam();
}