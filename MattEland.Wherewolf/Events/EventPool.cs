using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public static class EventPool
{
    private static readonly Dictionary<string, DealtCardEvent> _dealtEvents = new();
    
    public static DealtCardEvent DealtCardEvent(string slotName, GameRole slotRole)
    {
        string key = $"{slotName}-{slotRole}";
        
        if (_dealtEvents.TryGetValue(key, out DealtCardEvent? dealtCardEvent))
        {
            return dealtCardEvent;
        }

        DealtCardEvent newEvent = new(slotName, slotRole);
        _dealtEvents[key] = newEvent;

        return newEvent;
    }
}