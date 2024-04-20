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
            newState.AddEvent(new SoloWolfEvent(werewolves.First().Player!));
        } 
        else if (werewolves.Count > 1)
        {
            newState.AddEvent(new SawOtherWolvesEvent(werewolves.Select(w => w.Player!)));
        }

        return newState;
    }

    public override double Order => 1.0;
}