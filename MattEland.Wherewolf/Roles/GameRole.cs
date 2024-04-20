namespace MattEland.Wherewolf.Roles;

public abstract class GameRole
{
    public abstract Team Team { get; }
    public abstract string Name { get; }
    public abstract bool HasNightPhases { get; }

    public virtual IEnumerable<GamePhase> BuildNightPhases()
    {
        return Enumerable.Empty<GamePhase>();
    }
}