using MattEland.Wherewolf.Roles;
using MudBlazor;

namespace MattEland.Wherewolf.BlazorFrontEnd.Helpers;

public static class ThemeHelper
{
    public static string GetTeamTheme(this GameRole? role) 
        => GetTeamTheme(role?.GetTeam());    
    
    public static string GetTeamTheme(this GameRole role) 
        => GetTeamTheme(role.GetTeam());
    
    public static Color GetTeamColor(this GameRole role) 
        => role.GetTeam().GetTeamColor();    
    
    public static Color GetTeamColor(this Team? team) 
        => team == null 
            ? Color.Default
            : GetTeamColor(team.Value);    
    
    public static Color GetTeamColor(this Team team) 
        => team == Team.Villager 
            ? Color.Info 
            : Color.Error;
    
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
    
    public static string GetRoleIcon(this GameRole? role) => role switch
    {
        null => Icons.Material.Filled.QuestionMark,
        _ => GetRoleIcon(role.Value)
    };    
    
    public static string GetRoleIcon(this GameRole role) => role switch
    {
        GameRole.Villager => Icons.Material.Filled.Person,
        GameRole.Werewolf => Icons.Material.Filled.Bedtime,
        GameRole.Robber => Icons.Material.Filled.AttachMoney,
        GameRole.Insomniac => Icons.Material.Filled.Coffee,
        _ => Icons.Material.Filled.Error
    };
}