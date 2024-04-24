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

    public override string Description => $"{Player.Name} robbed {Target.Name} and saw their new role is {NewRole.Name}";
    public override bool IsPossibleInGameState(GameState state)
    {
        // Robbers can't rob themselves
        if (Target == Player) return false;

        // Only allow players who started as robbers to rob
        GameSlot robberSlot = state.GetPlayerSlot(Player);
        if (robberSlot.StartRole.Name != "Robber") return false;
        
        // Find the game state just prior to robbery
        while (state.CurrentPhase == null || state.CurrentPhase.Name == "Robber")
        {
            state = state.Parent!;
        }
        
        GameSlot targetSlot = state.GetPlayerSlot(Target);
        
        // Any setup that didn't start with the target having the robbed card cannot be considered
        return (targetSlot.CurrentRole.Name == this.NewRole.Name);
    }
}