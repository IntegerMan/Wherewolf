using MattEland.Wherewolf.Roles;
using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public static class DisplayHelpers
{
    public static string AsMarkdown(this GameRole role)
    {
        return role.GetTeam() switch
        {
            Team.Villager => $"[Blue]{role}[/]",
            Team.Werewolf => $"[Maroon]{role}[/]",
            _ => role.ToString()
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
        Table centerTable = new();
        centerTable.Title("[Yellow]Game Summary[/]");
        centerTable.AddColumn(string.Empty);
        List<string> startValues = ["[Cyan]Started as[/]"];
        foreach (var slot in gameState.Root.AllSlots)
        {
            string header = slot.GetSlotMarkup();
            centerTable.AddColumn(header);
            
            startValues.Add(slot.Role.AsMarkdown());
        }
        centerTable.AddRow(startValues.ToArray());
        
        List<string> endValues = ["[Cyan]Ended as[/]"];
        foreach (var slot in gameState.AllSlots)
        {
            endValues.Add(slot.Role.AsMarkdown());
        }
        centerTable.AddRow(endValues.ToArray());
        AnsiConsole.Write(centerTable);
    }

    public static string GetPlayerMarkup(this Player player) 
        => $"[{GetPlayerColor(player)}]{player.Name}[/]";

    public static string GetSlotMarkup(this GameSlot slot) 
        => slot.Player is null 
            ? $"[Gray50]{slot.Name}[/]" 
            : slot.Player.GetPlayerMarkup();

    public static string StylizeEventMessage(string message, IEnumerable<GameSlot> slots, IEnumerable<GameRole> roles)
    {
        foreach (var slot in slots)
        {
            message = message.Replace(slot.Name, slot.GetSlotMarkup(), StringComparison.OrdinalIgnoreCase);
        }
        
        foreach (var role in roles.Distinct())
        {
            message = message.Replace(role.ToString(), role.AsMarkdown(), StringComparison.OrdinalIgnoreCase);
        }

        return message;
    }
}