using MattEland.Wherewolf;
using MattEland.Wherewolf.Console;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Roles;
using Spectre.Console;

// Header
AnsiConsole.Write(new FigletText("Wherewolf?").Color(Color.Yellow));
AnsiConsole.MarkupLine("A social deduction simulation for computers, by [Cyan]Matt Eland[/]");
AnsiConsole.WriteLine();

// Game setup
GameSetup gameSetup = new();
gameSetup.AddPlayers(
        new Player("Matt", new HumanController()),
        new Player("Rufus", new RandomController()),
        new Player("Jimothy", new RandomController())
    );
gameSetup.AddRoles(
        new RobberRole(), 
        new InsomniacRole(), 
        new VillagerRole(), new VillagerRole(), 
        new WerewolfRole(), new WerewolfRole()
    );
    
GameState gameState = gameSetup.StartGame(new NonShuffler());
gameState = gameState.RunToEnd();

DisplayHelpers.DisplaySummaryTable(gameState);
AnsiConsole.WriteLine();

Tree eventTree = new("[Yellow]Game Events[/]");
foreach (var gameEvent in gameState.Events)
{
    AddGameEventNodeToTree(gameEvent, eventTree, gameState.AllSlots, gameState.Roles, gameState.Players);
}
AnsiConsole.Write(eventTree);
AnsiConsole.WriteLine();

// Display Player States
foreach (var player in gameSetup.Players)
{
    // Player divider
    Rule rule = new(player.GetPlayerMarkup());
    AnsiConsole.Write(rule);
    AnsiConsole.WriteLine();
    
    // Observed events tree
    Tree playerTree = new($"[Yellow]Observed Events for {player.GetPlayerMarkup()}[/]");
    
    foreach (var gameEvent in gameState.Events.Where(e => e.IsObservedBy(player)))
    {
        AddGameEventNodeToTree(gameEvent, playerTree, gameState.AllSlots, gameState.Roles, gameState.Players, includeObservedBy: false);
    }
    
    AnsiConsole.Write(playerTree);
    AnsiConsole.WriteLine();
    
    // Probabilities table
    RenderProbabilitiesTable(player, gameSetup, gameState, isStart: true);
    RenderProbabilitiesTable(player, gameSetup, gameState, isStart: false);
}

void AddGameEventNodeToTree(GameEvent evt, Tree tree, IEnumerable<GameSlot> slots, IEnumerable<GameRole> roles, IEnumerable<Player> players, bool includeObservedBy = true)
{
    // Make descriptions referencing slots or roles stand out more
    string description = DisplayHelpers.StylizeEventMessage(evt.Description, slots, roles);
    
    // Add the event node
    TreeNode eventNode = tree.AddNode($"[Cyan]{evt.GetType().Name}[/]: {description}");
    
    // Add observed by node as needed
    if (includeObservedBy)
    {
        string observedBy = string.Join(", ", players.Where(evt.IsObservedBy).Select(p => p.GetPlayerMarkup()));
        if (!string.IsNullOrWhiteSpace(observedBy))
        {
            eventNode.AddNode($"Observed by {observedBy}");
        }
    }
}

void RenderProbabilitiesTable(Player player, GameSetup setup, GameState state, bool isStart)
{
    Table probabilitiesTable = new();
    probabilitiesTable.Title($"[Yellow]{player.GetPlayerMarkup()}'s {(isStart ? "Start" : "Final")} Role Perceptions[/]");
    probabilitiesTable.AddColumn("Player");

    foreach (var role in setup.Roles.DistinctBy(r => r.Name).OrderBy(r => r.Name))
    {
        probabilitiesTable.AddColumn(role.AsMarkdown());
    }
    
    PlayerProbabilities probabilities = state.CalculateProbabilities(player);
    foreach (var otherSlot in state.AllSlots)
    {
        SlotRoleProbabilities slotProbabilities = isStart 
            ? probabilities.GetStartProbabilities(otherSlot) 
            : probabilities.GetCurrentProbabilities(otherSlot);

        List<string> values = [otherSlot.GetSlotMarkup()];
        foreach (var (_, probability) in slotProbabilities.Role.OrderBy(r => r.Key))
        {
            switch (probability)
            {
                case 0:
                    values.Add("[Gray39]0.0%[/]");
                    break;
                case 1:
                    values.Add("[White]100.0%[/]");
                    break;
                default:
                    values.Add($"[Grey62]{probability:P1}[/]");
                    break;
            }
        }
        probabilitiesTable.AddRow(values.ToArray());
    }
    
    AnsiConsole.Write(probabilitiesTable);
    AnsiConsole.WriteLine();
}