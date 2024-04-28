using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameSlot
{
    public GameSlot(string name, GameRole startRole, Player? player = null)
    {
        this.Name = name;
        this.Player = player;
        this.StartRole = startRole;
        this.BeginningOfPhaseRole = startRole;
        this.EndOfPhaseRole = startRole;
    }
    
    public GameSlot(GameSlot baseSlot)
    {
        this.Name = baseSlot.Name;
        this.Player = baseSlot.Player;
        this.StartRole = baseSlot.StartRole;
        this.BeginningOfPhaseRole = baseSlot.EndOfPhaseRole;
        this.EndOfPhaseRole = baseSlot.EndOfPhaseRole;
    }

    public string Name { get; init; }
    public Player? Player { get; init; }
    public GameRole StartRole { get; init; }
    public GameRole BeginningOfPhaseRole { get; init; }
    public GameRole EndOfPhaseRole { get; set; }
}