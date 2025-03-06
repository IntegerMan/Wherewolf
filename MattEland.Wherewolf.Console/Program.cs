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
    gameState.RunToEndOfNight(GameVoteManager.OnEndOfNight);
    
    return 0;
}
catch (Exception ex)
{
    AnsiConsole.WriteLine();
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return -1;
}