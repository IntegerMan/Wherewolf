using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

/// <summary>
/// An event that occurs when a werewolf player wakes during the night phase and discovers they
/// are the only werewolf player.
/// </summary>
public class LoneWolfEvent(Player player) : GameEvent
{
    public Player Player { get; } = player;

    public override Team? AssociatedTeam => Team.Werewolf;

    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description 
        => $"{Player.Name} saw that they were the only werewolf";

    public override bool IsPossibleInGameState(GameState state)
    {
        foreach (var player in state.Players)
        {
            GameRole startRole = state.Root[player.Name].Role;
            
            // If the involved player isn't a werewolf, they wouldn't have gotten this event
            if (player == Player && startRole.GetTeam() != Team.Werewolf)
            {
                return false;
            }

            // If other players were werewolves, this solo event wouldn't be possible
            if (player != Player && startRole.GetTeam() == Team.Werewolf)
            {
                return false;
            }
        }
        return true;
    }
}