using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

/// <summary>
/// This event occurs when the Robber robs a target. In that scenario they beg the target's role and get to see it while
/// the target has the role the robber player previously had. The target is not notified of the role swap.
/// </summary>
public class RobbedPlayerEvent : GameEvent
{
    public Player Player { get; }
    public Player Target { get; }
    public GameRole NewRole { get; }
    
    public RobbedPlayerEvent(Player robber, Player target, GameRole newRole)
    {
        Player = robber;
        Target = target;
        NewRole = newRole;
    }
    
    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description => $"{Player.Name} robbed {Target.Name} and saw their new role is {NewRole}";
    public override bool IsPossibleInGameState(GameState state)
    {
        // Robbers can't rob themselves
        if (Target == Player) return false;
        
        // Find the game state just prior to robbery
        GameState robbingState = state;
        while (robbingState.CurrentPhase is not { Name: "Robber" })
        {
            robbingState = robbingState.Parent!;
        }
        
        // Only allow players who started as robbers to rob
        GameSlot robberSlot = robbingState[Player.Name];
        if (robberSlot.StartRole != GameRole.Robber) return false;
        
        GameSlot targetSlot = robbingState[Target.Name];
        
        // Any setup that didn't start with the target having the robbed card cannot be considered
        if (targetSlot.BeginningOfPhaseRole != this.NewRole) return false;
        if (robberSlot.EndOfPhaseRole != this.NewRole) return false;
        if (targetSlot.EndOfPhaseRole != robberSlot.BeginningOfPhaseRole) return false;
        if (robberSlot.EndOfPhaseRole != targetSlot.BeginningOfPhaseRole) return false;

        return true;
    }
}