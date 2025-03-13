using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Game;

public class InsomniacSawFinalCardEvent : GameEvent
{
    internal InsomniacSawFinalCardEvent(string playerName, GameRole role)
    {
        PlayerName = playerName;
        Role = role;
    }

    public string PlayerName { get; }
    public GameRole Role { get; }

    public override bool IsObservedBy(Player player) => PlayerName == player.Name;

    public override string Description => Role == GameRole.Insomniac
        ? $"{PlayerName} saw that they were still the {Role}"
        : $"{PlayerName} saw that they were now the {Role}";
    
    public override Team? AssociatedTeam => Role.GetTeam();

    public override bool IsPossibleInGameState(GameState state) => state.ContainsEvent(this);
}