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

    public override string Description => Role.Name == "Insomniac"
        ? $"{Player.Name} saw that they were still the {Role.Name}"
        : $"{Player.Name} saw that they were now the {Role.Name}";
    
    public override bool IsPossibleInGameState(GameState state)
    {
        GameSlot playerSlot = state.GetPlayerSlot(Player);
        
        return playerSlot.StartRole.Name == "Insomniac" && playerSlot.EndOfPhaseRole.Name == this.Role.Name;
    }
}