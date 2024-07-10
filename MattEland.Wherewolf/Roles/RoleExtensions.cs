using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Roles;

public static class RoleExtensions
{
    public static IEnumerable<Type> GetNightPhasesForRole(this GameRole role)
    {
        switch (role)
        {
            case GameRole.Werewolf:
                yield return typeof(WerewolfNightPhase);
                break;
            case GameRole.Robber:
                yield return typeof(RobberNightPhase);
                break;
            case GameRole.Insomniac:
                yield return typeof(InsomniacNightPhase);
                break;
            case GameRole.Villager:
            default:
                yield break;
        } 
    }
    
    public static Team GetTeam(this GameRole role)
    {
        return role switch
        {
            GameRole.Werewolf => Team.Werewolf,
            _ => Team.Villager
        };
    }
}