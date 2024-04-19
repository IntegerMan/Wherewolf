namespace MattEland.Wherewolf.Roles;

public class WerewolfRole : GameRole
{
    public override Team Team => Team.Werewolf;
    public override string Name => "Werewolf";
}