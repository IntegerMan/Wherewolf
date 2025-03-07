using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class WerewolfNightPhase : GamePhase
{
    public override string Name => "Werewolves";

    public override void Run(GameState newState, Action<GameState> callback)
    {
        newState.AddEvent(new GamePhaseAnnouncedEvent(
            "Werewolves, wake up and look for each other. If there is only one werewolf, you may look a card in the center."));

        GameSlot[] werewolves = newState.PlayerSlots.Where(p => newState.GetStartRole(p).GetTeam() == Team.Werewolf)
            .ToArray();
        if (werewolves.Length == 1)
        {
            Player loneWolfPlayer = werewolves.First().Player!;

            newState.AddEvent(new LoneWolfEvent(loneWolfPlayer));

            // If the lone wolf is the only werewolf, let them pick which center card to look at
            GameSlot[] slots = newState.CenterSlots.ToArray();
            loneWolfPlayer.Controller.SelectLoneWolfCenterCard(slots, choice =>
            {
                newState.AddEvent(new LoneWolfLookedAtSlotEvent(loneWolfPlayer, choice));
                callback(newState);
            });
        }
        else
        {
            if (werewolves.Length > 1)
            {
                newState.AddEvent(new SawOtherWolvesEvent(werewolves.Select(w => w.Player!)));
            }

            callback(newState);
        }
    }

    public override double Order => 2.0;

    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        List<GameSlot> werewolves = priorState.PlayerSlots
            .Where(p => priorState.GetStartRole(p).GetTeam() == Team.Werewolf)
            .ToList();
        
        switch (werewolves.Count)
        {
            case 1:
            {
                // In cases where we have a lone wolf, we spawn a new possible state based on each card that could have been looked at
                Player loneWolfPlayer = werewolves.First().Player!;
                foreach (var centerSlot in priorState.CenterSlots)
                {
                    GameState lookedAtCenterCardState =
                        new(priorState, priorState.Support / priorState.CenterSlots.Length);
                    lookedAtCenterCardState.AddEvent(new LoneWolfEvent(loneWolfPlayer), broadcastToController: false);
                    lookedAtCenterCardState.AddEvent(new LoneWolfLookedAtSlotEvent(loneWolfPlayer, centerSlot),
                        broadcastToController: false);

                    yield return lookedAtCenterCardState;
                }

                break;
            }
            case > 1:
            {
                // In this case we had multiple werewolves, they do no action but we get an event
                GameState newState = new(priorState, priorState.Support);
                newState.AddEvent(new SawOtherWolvesEvent(werewolves.Select(w => w.Player!)),
                    broadcastToController: false);
                yield return newState;
                break;
            }
            default:
                // In this case we had no werewolves, so just advance without any events needed
                yield return new GameState(priorState, priorState.Support);
                break;
        }
    }
}