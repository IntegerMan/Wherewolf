namespace MattEland.Wherewolf.Roles;

public abstract class GameRole
{
    public abstract Team Team { get; }
    public abstract string Name { get; }
}