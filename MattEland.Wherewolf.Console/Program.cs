using MattEland.Wherewolf;
using MattEland.Wherewolf.Controllers;
using Spectre.Console;

// Header
AnsiConsole.Write(new FigletText("Wherewolf?").Color(Color.Yellow));
AnsiConsole.MarkupLine("A social deduction simulation for computers, by [Cyan]Matt Eland[/]");
AnsiConsole.WriteLine();

// Game setup
Game game = new();
game.AddPlayer(new Player("Matt", new HumanController()));
game.AddPlayer(new Player("Rufus", new RandomController()));
game.AddPlayer(new Player("Jimothy", new RandomController()));

