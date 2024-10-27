using System.Text;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public class HumanController : PlayerController
{
    public override void RanPhase(GamePhase phase, GameState gameState)
    {
        base.RanPhase(phase, gameState);

        AnsiConsole.WriteLine();
    }

    public override string SelectLoneWolfCenterCard(string[] centerSlotNames)
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select a card to look at from the center as the lone wolf")
            .AddChoices(centerSlotNames));
    }

    public override Player SelectRobberTarget(Player[] otherPlayers, GameState state, Player robber)
    {
        PlayerProbabilities probs = state.CalculateProbabilities(robber);
        SelectionPrompt<Player> prompt = new();
        prompt.Title(robber.GetPlayerMarkup() + ", select a player to rob");
        prompt.AddChoices(otherPlayers);
        prompt.HighlightStyle(new Style(foreground: Color.White));
        prompt.Converter = p =>
        {
            // TODO: This should use current probabilities as of before this phase, not the start probabilities
            IEnumerable<string> playerProbs = probs.GetStartProbabilities(state.GetPlayerSlot(p))
                .Where(r => r.Value.Probability > 0)
                .OrderByDescending(r => r.Value.Probability)
                .ThenBy(r => r.Key.ToString())
                .Select(r => $"{r.Key.AsMarkdown()} {r.Value.Probability:P2}");
            
            return $"{p.GetPlayerMarkup()} ({string.Join(", ", playerProbs)})";
        };

        return AnsiConsole.Prompt(prompt);
    }

    public override void ObservedEvent(GameEvent gameEvent, GameState state)
    {
        string message = DisplayHelpers.StylizeEventMessage(gameEvent.Description, state.AllSlots, state.Roles);
        AnsiConsole.MarkupLine(message);
    }

    public override GameRole GetInitialRoleClaim(Player player, GameState gameState)
    {
        SelectionPrompt<GameRole> prompt = new();
        GameRole startRole = gameState.GetStartRole(player);
        prompt.Title("What role are you claiming you started as? (Actual: " + startRole.AsMarkdown() + ")");
        prompt.HighlightStyle(new Style(foreground: Color.White));
        
        List<GameRole> roles = [startRole];
        roles.AddRange(gameState.Roles.Where(r => r != startRole).Distinct());
        prompt.AddChoices(roles);
        
        prompt.Converter = r =>
        {
            float winProb = VotingHelper.GetAssumedStartRoleVictoryProbabilities(player, gameState, r);
            return $"{r.AsMarkdown()} (Est. Avg. Probability: {string.Join(", ", 
                gameState.Players.Where(p => p != player)
                                 .Select(p => $"{p.GetPlayerMarkup()}: {RoleClaimVotingProbabilities.CalculateAverageBeliefProbability(gameState, player, p, r):P2}")
                )}, {winProb:P2} probable to win)";
        };
        
        return AnsiConsole.Prompt(prompt);
    }
    
    public override Player GetPlayerVote(Player votingPlayer, GameState state)
    {
        DisplayHelpers.RenderProbabilitiesTable(votingPlayer, state.Setup, state, isStart: true);
        DisplayHelpers.RenderProbabilitiesTable(votingPlayer, state.Setup, state, isStart: false);
        
        Dictionary<Player, float> probs = VotingHelper.GetVoteVictoryProbabilities(votingPlayer, state);
        
        SelectionPrompt<Player> prompt = new();
        prompt.Title("Who are you voting for?");
        prompt.AddChoices(state.Players.Where(p => p != votingPlayer));
        prompt.Converter = p =>
        {
            string claim = state.Events.OfType<StartRoleClaimedEvent>().First(c => c.Player == p).ClaimedRole
                .AsMarkdown();
            
            return $"{p.GetPlayerMarkup()} (Claims {claim}, {probs[p]:P2} likely to result in a win)";
        };

        return AnsiConsole.Prompt(prompt);
    }
}