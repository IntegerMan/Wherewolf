using System.Collections;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class PhaseContext
{
    public void AddEvent(GameEvent newEvent, bool broadcast)
    {
    }

    public IEnumerable<Player> PlayersStartingInRole(GameRole role)
    {
        throw new NotImplementedException();
    }

    public GameRole GetCurrentRole(Player player)
    {
        throw new NotImplementedException();
    }

    public GameSlot[] CenterSlots { get; }
    public GameSlot[] Players { get; }

    public Player[] OtherPlayers(Player player)
    {
        throw new NotImplementedException();
    }
}