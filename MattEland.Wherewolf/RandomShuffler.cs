using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class RandomShuffler : ISlotShuffler
{
    private readonly Random _random = new();

    public IEnumerable<GameRole> Shuffle(IEnumerable<GameRole> roles) 
        => roles.OrderBy(_ => _random.Next());
}