using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.BlazorFrontEnd.Helpers;

public static class ThemeHelper
{
    public static string GetTeamTheme(this GameRole? role) 
        => GetTeamTheme(role?.GetTeam());    
    
    public static string GetTeamTheme(this GameRole role) 
        => GetTeamTheme(role.GetTeam());

    public static string GetTeamTheme(this Team? team)
    {
        if (!team.HasValue)
        {
            return "mud-theme-dark";
        }

        return team.Value switch
        {
            Team.Villager => "mud-theme-info",
            Team.Werewolf => "mud-theme-error",
            _ => "mud-theme-warning"
        };
    }
}