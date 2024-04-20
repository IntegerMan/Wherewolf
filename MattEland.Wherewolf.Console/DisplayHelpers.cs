using MattEland.Wherewolf.Roles;
using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public static class DisplayHelpers
{
    public static string AsMarkdown(this GameRole role)
    {
        return role.Team switch
        {
            Team.Villager => $"[Blue]{role.Name}[/]",
            Team.Werewolf => $"[Maroon]{role.Name}[/]",
            _ => role.Name
        };
    }

    public static string GetPlayerColor(Player player)
    {
        return player.Order switch
        {
            0 => "HoneyDew2",
            1 => "chartreuse1",
            2 => "Purple",
            3 => "DarkOrange3",
            _ => "White"
        };
    }

    public static void DisplaySummaryTable(GameState gameState)
    {
        Table centerTable = new Table();
        centerTable.Title("[Yellow]Game Summary[/]");
        centerTable.AddColumn(string.Empty);
        List<string> values = new() { "[Cyan]Started as[/]"};
        int index = 0;
        foreach (var slot in gameState.AllSlots)
        {
            string header = slot.GetSlotMarkup();
    
            centerTable.AddColumn(header);

            values.Add(slot.StartRole.AsMarkdown());
        }
        centerTable.AddRow(values.ToArray());
        AnsiConsole.Write(centerTable);
    }

    public static string GetPlayerMarkup(this Player player) 
        => $"[{GetPlayerColor(player)}]{player.Name}[/]";

    public static string GetSlotMarkup(this GameSlot slot) 
        => slot.Player is null 
            ? $"[Gray50]{slot.Name}[/]" 
            : slot.Player.GetPlayerMarkup();
}