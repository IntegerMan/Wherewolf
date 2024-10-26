using MattEland.Wherewolf;
using MattEland.Wherewolf.Console;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;
using Spectre.Console;

try
{
    // Header
    AnsiConsole.Write(new FigletText("Wherewolf?").Color(Color.Yellow));
    AnsiConsole.MarkupLine("A social deduction simulation for computers, by [Cyan]Matt Eland[/]");
    AnsiConsole.WriteLine();

    // Game setup
    GameSetup gameSetup = new();
    gameSetup.AddPlayers(
        new Player("Rufus", new RandomController(new ClaimStartingRoleStrategy())),
        new Player("Jimothy", new RandomController(new ClaimStartingRoleStrategy())),
        new Player("Matt", new HumanController())
    );
    gameSetup.AddRoles(
        GameRole.Insomniac,
        GameRole.Werewolf,
        GameRole.Robber,
        GameRole.Villager,
        GameRole.Villager,
        GameRole.Werewolf
    );

    GameState gameState = gameSetup.StartGame(new NonShuffler());
    gameState = gameState.RunToEndOfNight();

    // Display Player States
    foreach (var player in gameSetup.Players)
    {
        if (player.Controller is HumanController)
        {
            // Player divider
            Rule rule = new(player.GetPlayerMarkup());
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
        }
    }

    // This will cause the voting to actually occur
    gameState = gameState.RunToEnd();

    // Display the game results
    GameResult result = gameState.GameResult!;
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[Bold]Game Results[/]");
    AnsiConsole.MarkupLine("Dead Players: " + string.Join(", ", result.DeadPlayers.Select(p => p.GetPlayerMarkup())));
    AnsiConsole.MarkupLine("Winning Team: " + result.WinningTeam.AsMarkdown());
    AnsiConsole.MarkupLine("Winning Players: " +
                           string.Join(", ", result.WinningPlayers.Select(p => p.GetPlayerMarkup())));
    AnsiConsole.WriteLine();

    // Post-Game Information
    DisplayHelpers.DisplaySummaryTable(gameState);
    AnsiConsole.WriteLine();
    return 0;
}
catch (Exception ex)
{
    AnsiConsole.WriteLine();
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return -1;
}