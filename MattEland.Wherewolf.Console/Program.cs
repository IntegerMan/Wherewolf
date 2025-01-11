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
    Random rand = new();
    GameSetup gameSetup = new();
    IRoleClaimStrategy roleClaimStrategy = new ClaimSafestRoleStrategy(rand);
    gameSetup.AddPlayers(
        new Player("Rufus", new RandomOptimalVoteController(roleClaimStrategy)),
        new Player("Jimothy", new RandomOptimalVoteController(roleClaimStrategy)),
        new Player("Matt", new RandomOptimalVoteController(roleClaimStrategy))
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
    
    // This will cause the voting and claims to actually occur
    gameState = gameState.RunToEnd();

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
        AnsiConsole.MarkupLine(DisplayHelpers.StylizeEventMessage(e.Description, gameState.AllSlots, gameState.Roles));
    }
    
    foreach (var claim in gameState.Claims)
    {
        AnsiConsole.MarkupLine(DisplayHelpers.StylizeEventMessage(claim.Description, gameState.AllSlots, gameState.Roles));
    }
    
    AnsiConsole.WriteLine();
    return 0;
}
catch (Exception ex)
{
    AnsiConsole.WriteLine();
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return -1;
}