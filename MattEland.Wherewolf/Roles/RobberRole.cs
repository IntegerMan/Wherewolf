using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Roles;

public class RobberRole : GameRole
{
    public override Team Team => Team.Villager;
    public override string Name => "Robber";
    public override bool HasNightPhases => true;

    public override IEnumerable<GamePhase> BuildNightPhases()
    {
        yield return new RobberNightPhase();
    }
}