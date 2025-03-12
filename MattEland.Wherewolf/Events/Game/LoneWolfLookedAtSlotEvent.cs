using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// This is a specialized event that occurs when a player is a lone wolf. I'm choosing to make this event specialized
/// for the lone wolf to help eliminate possible worlds at the deduction layer later. It may make more sense long-term
/// to merge all card looking abilities into a single event type.
/// </summary>
public class LoneWolfLookedAtSlotEvent : GameEvent
{
    /// <summary>
    /// This is a specialized event that occurs when a player is a lone wolf. I'm choosing to make this event specialized
    /// for the lone wolf to help eliminate possible worlds at the deduction layer later. It may make more sense long-term
    /// to merge all card looking abilities into a single event type.
    /// </summary>
    internal LoneWolfLookedAtSlotEvent(Player player, string slotName, GameRole role)
    {
        Player = player;
        SlotName = slotName;
        ObservedRole = role;
    }

    public Player Player { get; }
    public string SlotName { get; }
    public GameRole ObservedRole { get; }

    public override bool IsObservedBy(Player player) => Player == player;
    
    public override Team? AssociatedTeam => Team.Werewolf;

    public override string Description
        => $"{Player.Name} looked at {SlotName} since they were the only werewolf and saw a {ObservedRole}";

    public override bool IsPossibleInGameState(GameState state) => state.Events.Contains(this);
}