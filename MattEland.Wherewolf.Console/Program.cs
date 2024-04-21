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
        new VillagerRole(), new VillagerRole(), new VillagerRole(), new VillagerRole(), 
        new WerewolfRole(), new WerewolfRole()
    );
    
GameState gameState = gameSetup.StartGame();
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
    // Observed events tree
    PlayerState playerState = gameState.GetPlayerStates(player);
    Tree playerTree = new($"[Yellow]Observed Events for {player.GetPlayerMarkup()}[/]");
    
    foreach (var gameEvent in playerState.ObservedEvents)
    {
        AddGameEventNodeToTree(gameEvent, playerTree, gameState.AllSlots, gameState.Roles, gameState.Players, includeObservedBy: false);
    }
    
    AnsiConsole.Write(playerTree);
    AnsiConsole.WriteLine();
    
    // Probabilities table
    Table probabilitiesTable = new();
    probabilitiesTable.Title($"[Yellow]{player.GetPlayerMarkup()}'s Start Role Perceptions[/]");
    probabilitiesTable.AddColumn("Player");

    foreach (var role in gameSetup.Roles.DistinctBy(r => r.Name).OrderBy(r => r.Name))
    {
        probabilitiesTable.AddColumn(role.AsMarkdown());
    }
    
    var probabilities = playerState.Probabilities;
    foreach (var otherPlayer in gameSetup.Players)
    {
        GameSlot otherSlot = gameState.GetSlot(otherPlayer.Name);
        SlotRoleProbabilities slotProbabilities = probabilities.GetSlotProbabilities(otherSlot);
        
        List<string> values = [otherPlayer.GetPlayerMarkup()];
        foreach (var (_, probability) in slotProbabilities.OrderBy(r => r.Key))
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

// Display all possible worlds
Tree possibleStatesTree = new("[Yellow]Possible Worlds[/]");
foreach (var permutations in gameSetup.Permutations.GroupBy(p => string.Join(" ", p.State.PlayerSlots.Select(s => $"{s.GetSlotMarkup()}:{s.StartRole.AsMarkdown()}"))))
{
    string possibleForPlayers = "Possible for: ";
    int totalSupport = 0;
    foreach (var player in gameSetup.Players)
    {
        PlayerState playerState = gameState.GetPlayerStates(player);
        
        int support = permutations.Where(p => p.IsPossibleGivenPlayerState(playerState)).Sum(p => p.Support);
        if (support > 0)
        {
            possibleForPlayers += $"{player.GetPlayerMarkup()} ({support}) ";
            totalSupport += support;
        }
    }

    if (totalSupport > 0)
    {
        TreeNode permutationNode = possibleStatesTree.AddNode(permutations.Key);
        permutationNode.AddNode(possibleForPlayers);
    }
}
AnsiConsole.Write(possibleStatesTree);
AnsiConsole.WriteLine();

void AddGameEventNodeToTree(GameEvent evt, Tree tree, IEnumerable<GameSlot> slots, IEnumerable<GameRole> roles, IEnumerable<Player> players, bool includeObservedBy = true)
{
    // Make descriptions referencing slots or roles stand out more
    string description = evt.Description;
    foreach (var slot in slots)
    {
        description = description.Replace(slot.Name, slot.GetSlotMarkup(), StringComparison.OrdinalIgnoreCase);
    }
    foreach (var role in roles)
    {
        description = description.Replace(role.Name, role.AsMarkdown(), StringComparison.OrdinalIgnoreCase);
    }
    
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