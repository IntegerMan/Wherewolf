using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf;

public record PlayerState(Player Player, GameSetup GameSetup)
{
    private readonly List<GameEvent> _events = new();
    public IEnumerable<GameEvent> ObservedEvents => _events.AsReadOnly();

    public void AddEvents(IEnumerable<GameEvent> gameEvents)
    {
        _events.AddRange(gameEvents);
    }

    public void AddEvent(GameEvent gameEvent)
    {
        _events.Add(gameEvent);
    }

    public IEnumerable<CardProbability> CalculateStartingCardProbabilities(Player player)
    {
        if (player == Player)
        {
            yield return new CardProbability(_events.OfType<DealtCardEvent>().First().Role, 1);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}