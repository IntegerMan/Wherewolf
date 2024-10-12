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
        Tree playerTree = new($"[Yellow]Observed Events for {player.GetPlayerMarkup()}[/]");

        foreach (var gameEvent in gameState.Events.Where(e => e.IsObservedBy(player)))
        {
            AddGameEventNodeToTree(gameEvent, playerTree, gameState.AllSlots, gameState.Roles, gameState.Players,
                includeObservedBy: false);
        }

        AnsiConsole.Write(playerTree);
        AnsiConsole.WriteLine();

        // Probabilities table
        RenderProbabilitiesTable(player, gameSetup, gameState, isStart: true);
        RenderProbabilitiesTable(player, gameSetup, gameState, isStart: false);

        // Vote table
        RenderVoteWinPercents(player, gameState);
    }
}

// This will cause the voting to actually occur
gameState = gameState.RunToEnd();

// Display the game results
GameResult result = gameState.GameResult!;
AnsiConsole.WriteLine("Game Results");
AnsiConsole.MarkupLine("Dead Players: " + string.Join(", ", result.DeadPlayers.Select(p => p.GetPlayerMarkup())));
AnsiConsole.MarkupLine("Winning Team: " + result.WinningTeam);
AnsiConsole.MarkupLine("Winning Players: " + string.Join(", ", result.WinningPlayers.Select(p => p.GetPlayerMarkup())));
AnsiConsole.WriteLine();

// Post-Game Information
DisplayHelpers.DisplaySummaryTable(gameState);
AnsiConsole.WriteLine();

Tree eventTree = new("[Yellow]Game Events[/]");
foreach (var gameEvent in gameState.Events)
{
    AddGameEventNodeToTree(gameEvent, eventTree, gameState.AllSlots, gameState.Roles, gameState.Players);
}
AnsiConsole.Write(eventTree);
AnsiConsole.WriteLine();

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

    List<GameRole> orderedRoles = setup.Roles.Distinct().OrderBy(r => r.ToString()).ToList();
    foreach (var role in orderedRoles)
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
        foreach (var role in orderedRoles)
        {
            double probability = slotProbabilities[role];
            var probStr = GetFormattedPercent(probability);

            values.Add(probStr);
        }
        probabilitiesTable.AddRow(values.ToArray());
    }
    
    AnsiConsole.Write(probabilitiesTable);
    AnsiConsole.WriteLine();
}

string GetFormattedPercent(double value) 
    => value switch
    {
        0 => "[Gray39]0.0%[/]",
        1 => "[White]100.0%[/]",
        _ => $"[Grey62]{value:P1}[/]"
    };

void RenderVoteWinPercents(Player player1, GameState gameState1)
{
    Table voteTable = new();
    voteTable.Title($"{player1.GetPlayerMarkup()} Perceived Vote Win %'s");
    voteTable.AddColumns(gameState1.Players.Where(p => p != player1).Select(p => p.GetPlayerMarkup()).ToArray());
    var probabilities = gameState1.GetVoteVictoryProbabilities(player1);
    voteTable.AddRow(gameState1.Players.Where(p => p != player1).Select(p => GetFormattedPercent(probabilities[p])).ToArray());
    AnsiConsole.Write(voteTable);
}