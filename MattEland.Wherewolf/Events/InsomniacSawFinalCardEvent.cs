using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public class InsomniacSawFinalCardEvent : GameEvent
{
    public Player Player { get; }
    public GameRole Role { get; }

    public InsomniacSawFinalCardEvent(Player player, GameRole role)
    {
        this.Player = player;
        this.Role = role;
    }
    
    public override bool IsObservedBy(Player player) 
        => Player == player;

    public override string Description => Role == GameRole.Insomniac
        ? $"{Player.Name} saw that they were still the {Role}"
        : $"{Player.Name} saw that they were now the {Role}";
    
    public override bool IsPossibleInGameState(GameState state)
    {
        GameSlot playerSlot = state.GetPlayerSlot(Player);
        GameRole startRole = state.Root[Player.Name].Role;
        
        return startRole == GameRole.Insomniac && playerSlot.Role == this.Role;
    }
}