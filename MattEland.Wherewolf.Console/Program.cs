using MattEland.Wherewolf;
using MattEland.Wherewolf.Console;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;
using Spectre.Console;

// Header
AnsiConsole.Write(new FigletText("Wherewolf?").Color(Color.Yellow));
AnsiConsole.MarkupLine("A social deduction simulation for computers, by [Cyan]Matt Eland[/]");
AnsiConsole.WriteLine();

// Game setup
GameSetup gameSetup = new();
gameSetup.AddPlayers(
        new Player("Matt", new HumanController()),
        new Player("Rufus", new RandomController(new ClaimStartingRoleStrategy())),
        new Player("Jimothy", new RandomController(new ClaimStartingRoleStrategy()))
    );
gameSetup.AddRoles(
        GameRole.Robber, 
        GameRole.Insomniac, 
        GameRole.Villager, GameRole.Villager, 
        GameRole.Werewolf, GameRole.Werewolf
    );
    
GameState gameState = gameSetup.StartGame(new NonShuffler());
gameState = gameState.RunToEndOfNight();

Dictionary<Player, Player> votes = new();

// Display Player States
foreach (var player in gameSetup.Players)
{
    if (player.Controller is HumanController)
    {
        // Player divider
        Rule rule = new(player.GetPlayerMarkup());
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();

        // Observed events tree

        // Probabilities table
        //RenderProbabilitiesTable(player, gameSetup, gameState, isStart: true);
        //RenderProbabilitiesTable(player, gameSetup, gameState, isStart: false);

        // Vote table
        //RenderVoteWinPercents(player, gameState);
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
AnsiConsole.MarkupLine("Winning Players: " + string.Join(", ", result.WinningPlayers.Select(p => p.GetPlayerMarkup())));
AnsiConsole.WriteLine();

// Post-Game Information
DisplayHelpers.DisplaySummaryTable(gameState);
AnsiConsole.WriteLine();

