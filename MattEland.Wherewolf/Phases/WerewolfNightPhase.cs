using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Phases;

public class WerewolfNightPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        List<GameSlot> werewolves = newState.PlayerSlots.Where(p => p.StartRole.Team == Team.Werewolf).ToList();
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

    public override double Order => 1.0;
}