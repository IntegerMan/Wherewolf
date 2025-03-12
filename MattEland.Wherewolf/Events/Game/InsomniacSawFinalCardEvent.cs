using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class InsomniacSawFinalCardEvent : GameEvent
{
    internal InsomniacSawFinalCardEvent(Player player, GameRole role)
    {
        Player = player;
        Role = role;
    }

    public Player Player { get; }
    public GameRole Role { get; }

    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description => Role == GameRole.Insomniac
        ? $"{Player.Name} saw that they were still the {Role}"
        : $"{Player.Name} saw that they were now the {Role}";
    
    public override Team? AssociatedTeam => Role.GetTeam();
    
    public override bool IsPossibleInGameState(GameState state)
    {
        GameSlot playerSlot = state.GetSlot(Player);
        GameRole startRole = state.Root[Player.Name].Role;
        
        return startRole == GameRole.Insomniac && playerSlot.Role == Role;
    }
}