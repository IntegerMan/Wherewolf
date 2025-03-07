using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public static class GameVoteManager
{
    public static void OnEndOfNight(GameState gameState)
    {
        // Display Player States
        foreach (var player in gameState.Setup.Players)
        {
            if (player.Controller is HumanConsoleController)
            {
                // Player divider
                Rule rule = new(player.GetPlayerMarkup());
                AnsiConsole.Write(rule);
                AnsiConsole.WriteLine();
            }
        }
    
        // This will cause the voting and claims to actually occur
        gameState.RunToEnd(GameSummarizer.OnGameEnded);
    }
}