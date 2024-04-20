using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public interface ISlotShuffler
{
    IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles);
}