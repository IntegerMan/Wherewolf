using System.Collections.Concurrent;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public static class EventPool
{
    private static readonly ConcurrentDictionary<string, GameEvent> _events = new();

    private static GameEvent GetOrInstantiateEvent(string key, Func<GameEvent> eventFactory)
    {
        return _events.GetOrAdd(key, (Func<string, GameEvent>)Factory);
        
        GameEvent Factory(string _) => eventFactory();
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
        => GetOrInstantiateEvent(
            $"{nameof(LoneWolfLookedAtSlotEvent)}:{loneWolfPlayer.Name}:{choice.Name}:{choice.Role}",
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