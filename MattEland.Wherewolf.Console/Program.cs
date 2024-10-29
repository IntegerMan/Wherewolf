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
        new Player("Rufus", new RandomOptimalVoteController(new ClaimStartingRoleStrategy())),
        new Player("Jimothy", new RandomOptimalVoteController(new ClaimStartingRoleStrategy())),
        new Player("Matt", new HumanController())
    );
    gameSetup.AddRoles(
        GameRole.Villager,
        GameRole.Werewolf,
        GameRole.Villager,
        GameRole.Insomniac,
        GameRole.Werewolf,
        GameRole.Robber
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
    AnsiConsole.MarkupLine("Votes: " + string.Join(", ", result.Votes.OrderByDescending(kvp => kvp.Value)
                                                                     .ThenBy(kvp => kvp.Key.ToString())
                                                                     .Select(kvp => $"{kvp.Key.GetPlayerMarkup()}: {kvp.Value}")));
    AnsiConsole.MarkupLine("Dead Players: " + string.Join(", ", result.DeadPlayers.Select(p =>
        $"{p.GetPlayerMarkup()} ({gameState.GetPlayerSlot(p).Role.AsMarkdown()})")));
    AnsiConsole.MarkupLine($"Winning Team: {result.WinningTeam.AsMarkdown()}");
    AnsiConsole.MarkupLine($"Winning Players: {string.Join(", ", result.WinningPlayers.Select(p => p.GetPlayerMarkup()))}");
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