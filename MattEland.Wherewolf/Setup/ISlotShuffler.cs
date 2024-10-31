using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Setup;

public interface ISlotShuffler
{
    IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles);
}