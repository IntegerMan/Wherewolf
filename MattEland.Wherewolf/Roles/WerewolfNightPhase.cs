using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf.Roles;

public class WerewolfNightPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        List<GameSlot> werewolves = newState.PlayerSlots.Where(p => p.StartRole.Team == Team.Werewolf).ToList();
        if (werewolves.Count == 1)
        {
            newState.AddEvent(new SoloWolfEvent(werewolves.First().Player!));
        }

        return newState;
    }

    public override double Order => 1.0;
}