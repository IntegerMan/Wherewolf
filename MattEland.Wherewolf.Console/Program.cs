using MattEland.Wherewolf;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Roles;
using Spectre.Console;

// Header
AnsiConsole.Write(new FigletText("Wherewolf?").Color(Color.Yellow));
AnsiConsole.MarkupLine("A social deduction simulation for computers, by [Cyan]Matt Eland[/]");
AnsiConsole.WriteLine();

// Game setup
Game game = new();
game.AddPlayers(
        new Player("Matt", new HumanController()),
        new Player("Rufus", new RandomController()),
        new Player("Jimothy", new RandomController())
    );
game.AddRoles(
        new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), 
        new WerewolfRole(), new WerewolfRole()
    );
    
