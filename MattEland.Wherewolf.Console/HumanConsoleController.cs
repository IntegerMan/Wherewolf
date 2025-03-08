using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public class HumanConsoleController : PlayerController
{
    public override void SelectLoneWolfCenterCard(GameSlot[] centerSlots, Action<GameSlot> callback)
    {
        GameSlot choice = AnsiConsole.Prompt(new SelectionPrompt<GameSlot>()
            .Title("Select a card to look at from the center as the lone wolf")
            .AddChoices(centerSlots)
            .UseConverter(s => s.Name));
        
        callback(choice);
    }

    public override void SelectRobberTarget(Player[] otherPlayers, GameState state, Player robber, Action<Player> callback)
    {
        PlayerProbabilities probs = state.CalculateProbabilities(robber);
        SelectionPrompt<Player> prompt = new();
        prompt.Title($"{robber.GetPlayerMarkup()}, select a player to rob");
        prompt.AddChoices(otherPlayers);
        prompt.HighlightStyle(new Style(foreground: Color.White));
        prompt.Converter = p =>
        {
            IEnumerable<string> playerProbs = probs.GetStartProbabilities(state.GetSlot(p))
                .Where(r => r.Value.Probability > 0)
                .OrderByDescending(r => r.Value.Probability)
                .ThenBy(r => r.Key.ToString())
                .Select(r => $"{r.Key.AsMarkdown()} {r.Value.Probability:P0}");
            
            return $"{p.GetPlayerMarkup()} ({string.Join(", ", playerProbs)})";
        };

        Player choice = AnsiConsole.Prompt(prompt);
        callback(choice);
    }

    public override void ObservedEvent(GameEvent gameEvent, GameState state)
    {
        string message = DisplayHelpers.StylizeEventMessage(gameEvent.Description, state.AllSlots, state.Roles);
        AnsiConsole.MarkupLine(message);
    }

    public override void GetInitialRoleClaim(Player player, GameState gameState, Action<GameRole> callback)
    {
        SelectionPrompt<GameRole> prompt = new();
        GameRole startRole = gameState.GetStartRole(player);
        prompt.Title($"What role are you claiming you started as? (Actual: {startRole.AsMarkdown()})");
        prompt.HighlightStyle(new Style(foreground: Color.White));
        
        List<GameRole> roles = [startRole];
        roles.AddRange(gameState.Roles.Where(r => r != startRole).Distinct());
        prompt.AddChoices(roles);
        
        IEnumerable<Player> otherPlayers = gameState.Players.Where(p => p != player);
        
        /*
        prompt.Converter = r =>
        {
            float winProb = 0; //VotingHelper.GetRoleClaimWinProbabilityPerception(player, gameState, r);

            return $"{r.AsMarkdown()} (Est. Avg. Probability: {string.Join(", ", otherPlayers
                                 .Select(p => $"{p.GetPlayerMarkup()}: {RoleClaimVotingProbabilities.CalculateAverageBeliefProbability(gameState, player, p, r):P0}")
                )}, {winProb:P0} probable to win)";
        };
        */
        
        GameRole choice = AnsiConsole.Prompt(prompt);
        callback(choice);
    }
    
    public override void GetPlayerVote(Player votingPlayer, GameState state, PlayerProbabilities playerProbs, Dictionary<Player, double> victoryProbs, Action<Player> callback)
    {
        SelectionPrompt<Player> prompt = new();
        prompt.Title("Who are you voting for?");
        prompt.AddChoices(state.Players.Where(p => p != votingPlayer));
        prompt.Converter = p =>
        {
            string claim = state.Claims.OfType<StartRoleClaimedEvent>().First(c => c.Player == p).ClaimedRole
                .AsMarkdown();
            
            string current = string.Join(", ", playerProbs.GetCurrentProbabilities(state.GetSlot(p))
                .Where(kvp => kvp.Value.Probability > 0)
                .OrderByDescending(kvp => kvp.Value.Probability)
                .Select(kvp => $"{kvp.Value.Probability:P0} {kvp.Key.AsMarkdown()}"));
            
            return $"{p.GetPlayerMarkup()} (Claims {claim}, ({current}), {victoryProbs[p]:P0} win chance)";
        };

        Player choice = AnsiConsole.Prompt(prompt);
        callback(choice);
    }
}