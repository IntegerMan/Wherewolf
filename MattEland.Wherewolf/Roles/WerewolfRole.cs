using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Roles;

public class WerewolfRole : GameRole
{
    public override Team Team => Team.Werewolf;
    public override string Name => "Werewolf";
    public override bool HasNightPhases => true;

    public override IEnumerable<GamePhase> BuildNightPhases()
    {
        yield return new WerewolfNightPhase();
    }
}