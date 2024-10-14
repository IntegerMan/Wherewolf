using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Setup;

public class NonShuffler : ISlotShuffler
{
    public IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles) 
        => roles.ToArray();
}