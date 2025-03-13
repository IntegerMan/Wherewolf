using System.Collections.Concurrent;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events;

public static class EventPool
{
    // NOTE: If this becomes too frequently locked or too large to sift through, we could use separate dictionaries per event type
    private static readonly ConcurrentDictionary<string, GameEvent> Events = new();

    private static GameEvent GetOrInstantiateEvent(string key, Func<GameEvent> eventFactory)
    {
        return Events.GetOrAdd(key, (Func<string, GameEvent>)Factory);
        
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

    public static GameEvent InsomniacSawCard(string playerName, GameRole insomniacRole)
        => GetOrInstantiateEvent($"{nameof(InsomniacSawFinalCardEvent)}:{playerName}:{insomniacRole}",
            () => new InsomniacSawFinalCardEvent(playerName, insomniacRole));

    public static GameEvent LoneWolf(string loneWolfPlayerName, GameSlot choice)
        => GetOrInstantiateEvent(
            $"{nameof(LoneWolfLookedAtSlotEvent)}:{loneWolfPlayerName}:{choice.Name}:{choice.Role}",
            () => new LoneWolfLookedAtSlotEvent(loneWolfPlayerName, choice.Name, choice.Role));

    public static GameEvent WolfTeam(IEnumerable<Player> wolves)
        => GetOrInstantiateEvent($"{nameof(SawOtherWolvesEvent)}:{string.Join(",", wolves.Select(p => p.Name))}",
            () => new SawOtherWolvesEvent(wolves));

    public static GameEvent Robbed(string robberPlayer, string targetPlayer, GameRole stolenRole)
        => GetOrInstantiateEvent($"{nameof(RobbedPlayerEvent)}:{robberPlayer}:{targetPlayer}:{stolenRole}",
            () => new RobbedPlayerEvent(robberPlayer, targetPlayer, stolenRole));

    public static GameEvent VotedOut(Player deadPlayer, GameRole finalRole)
        => GetOrInstantiateEvent($"{nameof(VotedOutEvent)}:{deadPlayer.Name}:{finalRole}",
            () => new VotedOutEvent(deadPlayer, finalRole));
}