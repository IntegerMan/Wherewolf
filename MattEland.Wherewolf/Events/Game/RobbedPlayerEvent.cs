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
    public override bool IsPossibleInGameState(GameState state) => state.ContainsEvent(this);
}