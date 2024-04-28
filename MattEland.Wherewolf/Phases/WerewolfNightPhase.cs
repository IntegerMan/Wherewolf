using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class WerewolfNightPhase : GamePhase
{
    public override string Name => "Werewolves";
    
    public override GameState Run(GameState newState)
    {
        List<GameSlot> werewolves = newState.PlayerSlots.Where(p => p.StartRole.GetTeam() == Team.Werewolf).ToList();
        if (werewolves.Count == 1)
        {
            Player loneWolfPlayer = werewolves.First().Player!;
            
            newState.AddEvent(new LoneWolfEvent(loneWolfPlayer));
            
            // If the lone wolf is the only werewolf, let them pick which center card to look at
            string centerSlotChoice = loneWolfPlayer.Controller.SelectLoneWolfCenterCard(newState.CenterSlots.Select(c => c.Name).ToArray());
            GameSlot centerSlot = newState.GetSlot(centerSlotChoice);
            newState.AddEvent(new LoneWolfLookedAtSlotEvent(loneWolfPlayer, centerSlot));
        } 
        else if (werewolves.Count > 1)
        {
            newState.AddEvent(new SawOtherWolvesEvent(werewolves.Select(w => w.Player!)));
        }

        return newState;
    }

    public override double Order => 2.0;
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        List<GameSlot> werewolves = priorState.PlayerSlots.Where(p => p.StartRole.GetTeam() == Team.Werewolf).ToList();
        if (werewolves.Count == 1)
        {
            // In cases where we have a lone wolf, we spawn a new possible state based on each card that could have been looked at
            Player loneWolfPlayer = werewolves.First().Player!;
            foreach (var centerSlot in priorState.CenterSlots)
            {
                GameState lookedAtCenterCardState = new(priorState);
                lookedAtCenterCardState.AddEvent(new LoneWolfEvent(loneWolfPlayer), broadcastToController: false);
                lookedAtCenterCardState.AddEvent(new LoneWolfLookedAtSlotEvent(loneWolfPlayer, centerSlot), broadcastToController: false);

                yield return lookedAtCenterCardState;
            }
        } 
        else if (werewolves.Count > 1)
        {
            // In this case we had multiple werewolves, they do no action but we get an event
            GameState newState = new(priorState);
            newState.AddEvent(new SawOtherWolvesEvent(werewolves.Select(w => w.Player!)), broadcastToController: false);
            yield return newState;
        }
        else
        {
            // In this case we had no werewolves, so just advance without any events needed
            yield return new GameState(priorState);
        }
    }
}