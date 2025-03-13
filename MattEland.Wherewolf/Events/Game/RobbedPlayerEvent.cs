using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// This event occurs when the Robber robs a target. In that scenario they beg the target's role and get to see it while
/// the target has the role the robber player previously had. The target is not notified of the role swap.
/// </summary>
public class RobbedPlayerEvent : GameEvent
{
    /// <summary>
    /// This event occurs when the Robber robs a target. In that scenario they beg the target's role and get to see it while
    /// the target has the role the robber player previously had. The target is not notified of the role swap.
    /// </summary>
    internal RobbedPlayerEvent(string robberName, string targetName, GameRole newRole)
    {
        PlayerName = robberName;
        TargetName = targetName;
        NewRole = newRole;
    }

    public string PlayerName { get; }
    public string TargetName { get; }
    public GameRole NewRole { get; }
    
    public override Team? AssociatedTeam => NewRole.GetTeam();

    public override bool IsObservedBy(Player player) 
        => PlayerName == player.Name;

    public override string Description => $"{PlayerName} robbed {TargetName} and saw their new role is {NewRole}";
    
    public override bool IsPossibleInGameState(GameState state)
    {
        if (state.ContainsEvent(this)) return true;
        
        // Robbers can't rob themselves
        if (TargetName == PlayerName) return false;
        
        // Find the game state just prior to robbery
        GameState beforeRobState = state;
        while (beforeRobState.CurrentPhase is not { Name: "Robber" })
        {
            beforeRobState = beforeRobState.Parent!;
        }

        GameState afterRobState = state;
        while (afterRobState.Parent != beforeRobState)
        {
            afterRobState = afterRobState.Parent!;
        }
        
        // Only allow players who started as robbers to rob
        if (state.GetStartRole(PlayerName) != GameRole.Robber) return false;

        GameRole robbersOldRole = beforeRobState[PlayerName].Role;
        
        // Any setup that didn't start with the target having the robbed card cannot be considered
        if (beforeRobState[TargetName].Role != NewRole) return false;
        if (beforeRobState[PlayerName].Role != robbersOldRole) return false;
        if (afterRobState[PlayerName].Role != NewRole) return false;
        if (afterRobState[TargetName].Role != robbersOldRole) return false;

        return true;
    }

}