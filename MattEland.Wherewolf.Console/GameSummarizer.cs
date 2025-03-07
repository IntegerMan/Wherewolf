using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public static class GameSummarizer 
{
    public static void OnGameEnded(GameState gameState)
    {
        // Display the game results
        GameResult result = gameState.GameResult!;
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[Bold]Game Results[/]");
        AnsiConsole.MarkupLine("Votes: " + string.Join(", ", result.Votes.OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => kvp.Key.ToString())
            .Select(kvp => $"{kvp.Key.GetPlayerMarkup()}: {kvp.Value}")));
        AnsiConsole.MarkupLine("Dead Players: " + string.Join(", ", result.DeadPlayers.Select(p =>
            $"{p.GetPlayerMarkup()} ({gameState.GetSlot(p).Role.AsMarkdown()})")));
        AnsiConsole.MarkupLine($"Winning Team: {result.WinningTeam.AsMarkdown()}");
        AnsiConsole.MarkupLine(
            $"Winning Players: {string.Join(", ", result.WinningPlayers.Select(p => $"{p.GetPlayerMarkup()} ({gameState.GetSlot(p).Role.AsMarkdown()})"))
            }");

        AnsiConsole.WriteLine();

        // Post-Game Information  
        DisplayHelpers.DisplaySummaryTable(gameState);

        foreach (var e in gameState.Events)
        {
            AnsiConsole.MarkupLine(DisplayHelpers.StylizeEventMessage(e.Description, gameState.AllSlots, gameState.Setup.Roles));
        }
    
        foreach (var claim in gameState.Claims)
        {
            AnsiConsole.MarkupLine(DisplayHelpers.StylizeEventMessage(claim.Description, gameState.AllSlots, gameState.Setup.Roles));
        }
    
        AnsiConsole.WriteLine();
    }
}