using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

/// <summary>
/// This is a specialized event that occurs when a player is a lone wolf. I'm choosing to make this event specialized
/// for the lone wolf to help eliminate possible worlds at the deduction layer later. It may make more sense long-term
/// to merge all card looking abilities into a single event type.
/// </summary>
public class LoneWolfLookedAtSlotEvent : GameEvent
{
    public Player Player { get; }
    public GameSlot Slot { get; }
    public string ObservedRole { get; }

    public LoneWolfLookedAtSlotEvent(Player player, GameSlot slot)
    {
        this.Player = player;
        this.Slot = slot;
        this.ObservedRole = slot.CurrentRole.Name;
    }

    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description
        => $"{Player.Name} looked at {Slot.Name} since they were the only werewolf and saw a {ObservedRole}";

    public override bool IsPossibleInGameState(GameState state)
    {
        // This event is only possible with a solo wolf
        if (state.PlayerSlots.Count(p => p.StartRole.Team == Team.Werewolf) != 1)
        {
            return false;
        }
        
        // If the executing player was not a werewolf, this event is not possible
        if (state.GetPlayerSlot(Player).StartRole.Team != Team.Werewolf)
        {
            return false;
        }
        
        // In the game state, the current role needs to be the one the event recorded seeing
        return state.GetSlot(Slot.Name).CurrentRole.Name == ObservedRole;
    }
}