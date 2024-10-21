using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;
using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public static class DisplayHelpers
{
    public static string AsMarkdown(this GameRole role)
    {
        return role.GetTeam() switch
        {
            Team.Villager => $"[Blue]{role}[/]",
            Team.Werewolf => $"[Maroon]{role}[/]",
            _ => role.ToString()
        };
    }    
    
    public static string AsMarkdown(this Team team)
    {
        return team switch
        {
            Team.Villager => $"[Blue]{team}[/]",
            Team.Werewolf => $"[Maroon]{team}[/]",
            _ => team.ToString()
        };
    }

    public static string GetPlayerColor(Player player)
    {
        return player.Order switch
        {
            0 => "HoneyDew2",
            1 => "chartreuse1",
            2 => "Purple",
            3 => "DarkOrange3",
            _ => "White"
        };
    }

    public static void DisplaySummaryTable(GameState gameState)
    {
        Table centerTable = new();
        centerTable.Title("[Yellow]Game Summary[/]");
        centerTable.AddColumn(string.Empty);
        List<string> startValues = ["[Cyan]Started as[/]"];
        foreach (var slot in gameState.Root.AllSlots)
        {
            string header = slot.GetSlotMarkup();
            centerTable.AddColumn(header);
            
            startValues.Add(slot.Role.AsMarkdown());
        }
        centerTable.AddRow(startValues.ToArray());
        
        List<string> endValues = ["[Cyan]Ended as[/]"];
        foreach (var slot in gameState.AllSlots)
        {
            endValues.Add(slot.Role.AsMarkdown());
        }
        centerTable.AddRow(endValues.ToArray());
        AnsiConsole.Write(centerTable);
    }

    public static string GetPlayerMarkup(this Player player) 
        => $"[{GetPlayerColor(player)}]{player.Name}[/]";

    public static string GetSlotMarkup(this GameSlot slot) 
        => slot.Player is null 
            ? $"[Gray50]{slot.Name}[/]" 
            : slot.Player.GetPlayerMarkup();

    public static string StylizeEventMessage(string message, IEnumerable<GameSlot> slots, IEnumerable<GameRole> roles)
    {
        foreach (var slot in slots)
        {
            message = message.Replace(slot.Name, slot.GetSlotMarkup(), StringComparison.OrdinalIgnoreCase);
        }
        
        foreach (var role in roles.Distinct())
        {
            message = message.Replace(role.ToString(), role.AsMarkdown(), StringComparison.OrdinalIgnoreCase);
        }
        
        message = message.Replace("Werewolves", "[Maroon]Werewolves[/]", StringComparison.OrdinalIgnoreCase);
        message = message.Replace("Everyone", "[White Bold]Everyone[/]", StringComparison.OrdinalIgnoreCase);

        return message;
    }
    
    public static void RenderProbabilitiesTable(Player player, GameSetup setup, GameState state, bool isStart)
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
    
    private static string GetFormattedPercent(double value) 
        => value switch
        {
            0 => "[Gray39]0.0%[/]",
            1 => "[White]100.0%[/]",
            _ => $"[Grey62]{value:P1}[/]"
        };

    public static void RenderVoteWinPercents(Player player1, GameState gameState1)
    {
        Table voteTable = new();
        voteTable.Title($"{player1.GetPlayerMarkup()} Perceived Vote Win %'s");
        voteTable.AddColumns(gameState1.Players.Where(p => p != player1).Select(p => p.GetPlayerMarkup()).ToArray());
        var probabilities = VotingHelper.GetVoteVictoryProbabilities(player1, gameState1);
        voteTable.AddRow(gameState1.Players.Where(p => p != player1).Select(p => GetFormattedPercent(probabilities[p])).ToArray());
        AnsiConsole.Write(voteTable);
    }

    public static void RenderObservedEvents(Player player, GameState state)
    {
        Tree playerTree = new($"[Yellow]Observed Events for {player.GetPlayerMarkup()}[/]");

        foreach (var gameEvent in state.Events.Where(e => e.IsObservedBy(player)))
        {
            AddGameEventNodeToTree(gameEvent, playerTree, state.AllSlots, state.Roles, state.Players,
                includeObservedBy: false);
        }

        AnsiConsole.Write(playerTree);
        AnsiConsole.WriteLine();

    }
    
    private static void AddGameEventNodeToTree(GameEvent evt, Tree tree, IEnumerable<GameSlot> slots, IEnumerable<GameRole> roles, IEnumerable<Player> players, bool includeObservedBy = true)
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

    public static void DisplayPostGameEvents(GameState state)
    {
        Tree eventTree = new("[Yellow]Game Events[/]");
        foreach (var gameEvent in state.Events)
        {
            AddGameEventNodeToTree(gameEvent, eventTree, state.AllSlots, state.Roles, state.Players);
        }
        AnsiConsole.Write(eventTree);
        AnsiConsole.WriteLine();
    }
}