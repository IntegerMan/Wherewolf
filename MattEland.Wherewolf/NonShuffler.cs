using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class NonShuffler : ISlotShuffler
{
    public IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles) 
        => roles.ToArray();
}