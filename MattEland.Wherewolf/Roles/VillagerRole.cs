namespace MattEland.Wherewolf.Roles;

public class VillagerRole : GameRole
{
    public override Team Team => Team.Villager;
    public override string Name => "Villager";
    public override bool HasNightPhases => false;
}