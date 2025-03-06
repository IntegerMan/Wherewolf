using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// This event occurs when the Robber robs a target. In that scenario they beg the target's role and get to see it while
/// the target has the role the robber player previously had. The target is not notified of the role swap.
/// </summary>
public class RobbedPlayerEvent(Player robber, Player target, GameRole newRole) : GameEvent
{
    public Player Player { get; } = robber;
    public Player Target { get; } = target;
    public GameRole NewRole { get; } = newRole;
    
    public override Team? AssociatedTeam => NewRole.GetTeam();

    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description => $"{Player.Name} robbed {Target.Name} and saw their new role is {NewRole}";
    public override bool IsPossibleInGameState(GameState state)
    {
        // Robbers can't rob themselves
        if (Target == Player) return false;
        
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
        if (state.GetStartRole(Player) != GameRole.Robber) return false;

        GameRole robbersOldRole = beforeRobState[Player.Name].Role;
        
        // Any setup that didn't start with the target having the robbed card cannot be considered
        if (beforeRobState[Target.Name].Role != NewRole) return false;
        if (beforeRobState[Player.Name].Role != robbersOldRole) return false;
        if (afterRobState[Player.Name].Role != NewRole) return false;
        if (afterRobState[Target.Name].Role != robbersOldRole) return false;

        return true;
    }
}