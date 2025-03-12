using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public static class EventPool
{
    private static readonly Dictionary<string, GameEvent> _events = new();
    private static readonly Lock _lock = new();

    private static GameEvent GetOrInstantiateEvent(string key, Func<GameEvent> eventFactory)
    {
        // This lock should ensure we don't have two threads trying to create the same event
        lock (_lock)
        {
            if (_events.TryGetValue(key, out var gameEvent)) return gameEvent;
            GameEvent newEvent = eventFactory();
            _events[key] = newEvent;

            return newEvent;
        }
    }

    public static GameEvent DealtCardEvent(string slotName, GameRole slotRole)
        => GetOrInstantiateEvent($"{nameof(DealtCardEvent)}:{slotName}:{slotRole}",
            () => new DealtCardEvent(slotName, slotRole));

    public static GameEvent GameOver(Team winningTeam)
        => GetOrInstantiateEvent($"{nameof(GameOverEvent)}:{winningTeam}",
            () => new GameOverEvent(winningTeam));

    public static GameEvent Announcement(string message, GameRole? role = null)
        => GetOrInstantiateEvent($"{nameof(GamePhaseAnnouncedEvent)}:{message}:{role}",
            () => new GamePhaseAnnouncedEvent(message, role));

    public static GameEvent MakeSocialClaimsNow()
        => GetOrInstantiateEvent(nameof(MakeSocialClaimsNowEvent),
            () => new MakeSocialClaimsNowEvent());

    public static GameEvent InsomniacSawCard(Player insomniacPlayer, GameRole insomniacRole)
        => GetOrInstantiateEvent(nameof(InsomniacSawFinalCardEvent),
            () => new InsomniacSawFinalCardEvent(insomniacPlayer, insomniacRole));

    public static GameEvent LoneWolf(Player loneWolfPlayer, GameSlot choice)
        => GetOrInstantiateEvent($"{nameof(LoneWolfLookedAtSlotEvent)}:{loneWolfPlayer.Name}:{choice.Name}:{choice.Role}",
            () => new LoneWolfLookedAtSlotEvent(loneWolfPlayer, choice.Name, choice.Role));

    public static GameEvent WolfTeam(IEnumerable<Player> wolves)
        => GetOrInstantiateEvent($"{nameof(SawOtherWolvesEvent)}:{string.Join(",", wolves.Select(p => p.Name))}",
            () => new SawOtherWolvesEvent(wolves));

    public static GameEvent Robbed(Player robberPlayer, Player targetPlayer, GameRole stolenRole)
        => GetOrInstantiateEvent($"{nameof(RobbedPlayerEvent)}:{robberPlayer.Name}:{targetPlayer.Name}:{stolenRole}",
            () => new RobbedPlayerEvent(robberPlayer, targetPlayer, stolenRole));

    public static GameEvent VotedOut(Player deadPlayer, GameRole finalRole)
        => GetOrInstantiateEvent($"{nameof(VotedOutEvent)}:{deadPlayer.Name}:{finalRole}",
            () => new VotedOutEvent(deadPlayer, finalRole));
}