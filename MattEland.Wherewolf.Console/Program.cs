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
    
GameState gameState = game.StartGame();

Table centerTable = new Table();
centerTable.Title("[Yellow]Game Summary[/]");
centerTable.AddColumn(string.Empty);
List<string> values = new() { "[Cyan]Started as[/]"};
foreach (var slot in gameState.AllSlots)
{
    if (slot.Player is null)
    {
        centerTable.AddColumn($"[Cyan]{slot.Name}[/]");
    }
    else
    {
        centerTable.AddColumn($"[Orange1]{slot.Name}[/]");
    }

    values.Add(GetRoleMarkdown(slot.StartRole));
}
centerTable.AddRow(values.ToArray());
AnsiConsole.Write(centerTable);

string GetRoleMarkdown(GameRole role)
{
    return role.Team switch
    {
        Team.Villager => $"[Blue]{role.Name}[/]",
        Team.Werewolf => $"[Red1]{role.Name}[/]",
        _ => role.Name
    };
}