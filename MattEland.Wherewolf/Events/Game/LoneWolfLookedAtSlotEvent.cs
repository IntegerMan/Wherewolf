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

    public override bool IsObservedBy(Player player) 
        => Player == player;
    
    public override Team? AssociatedTeam => Team.Werewolf;

    public override string Description
        => $"{Player.Name} looked at {SlotName} since they were the only werewolf and saw a {ObservedRole}";

    public override bool IsPossibleInGameState(GameState state)
    {
        // This event is only possible with a solo wolf
        if (state.PlayerSlots.Count(p => state.GetStartRole(p).GetTeam() == Team.Werewolf) != 1)
        {
            return false;
        }
        
        // If the executing player was not a werewolf, this event is not possible
        if (state.GetStartRole(Player).GetTeam() != Team.Werewolf)
        {
            return false;
        }

        GameState? wwPhaseState = state;
        while (wwPhaseState is { CurrentPhase: not { Name: "Werewolves" } })
        {
            wwPhaseState = wwPhaseState.Parent;
        }
        
        // If we didn't find a werewolf phase, we're looking at a phase before the WW phase, so this should be true
        if (wwPhaseState == null)
        {
            return true;
        }
        
        // In the game state, the current role needs to be the one the event recorded seeing
        return wwPhaseState[SlotName].Role == ObservedRole;
    }
}