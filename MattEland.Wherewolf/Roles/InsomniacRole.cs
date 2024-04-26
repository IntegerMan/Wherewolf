using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Roles;

public class InsomniacRole : GameRole
{
    public override Team Team => Team.Villager;
    public override string Name => "Insomniac";
    public override bool HasNightPhases => true;

    public override IEnumerable<GamePhase> BuildNightPhases()
    {
        yield return new InsomniacNightPhase();
    }
}