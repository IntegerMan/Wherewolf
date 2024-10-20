using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using Spectre.Console;

namespace MattEland.Wherewolf.Console;

public class HumanController : PlayerController
{
    public override string SelectLoneWolfCenterCard(string[] centerSlotNames)
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select a card to look at from the center as the lone wolf")
            .AddChoices(centerSlotNames));
    }

    public override string SelectRobberTarget(string[] otherPlayerNames)
    {
        SelectionPrompt<string> prompt = new SelectionPrompt<string>();
        prompt.Title("Select a player to rob");
        prompt.AddChoices(otherPlayerNames);
        prompt.HighlightStyle(new Style(foreground: Color.White));
        // TODO: prompt.Converter = p => p.GetPlayerMarkup();

        return AnsiConsole.Prompt(prompt);
    }

    public override void ObservedEvent(GameEvent gameEvent, GameState state)
    {
        string message = DisplayHelpers.StylizeEventMessage(gameEvent.Description, state.AllSlots, state.Roles);
        AnsiConsole.MarkupLine(message);
    }

    public override Player GetPlayerVote(Player votingPlayer, GameState gameState)
    {
        Dictionary<Player, float> probs = VotingHelper.GetVoteVictoryProbabilities(votingPlayer, gameState);
        
        SelectionPrompt<Player> prompt = new();
        prompt.Title("Who are you voting for?");
        prompt.AddChoices(gameState.Players.Where(p => p != votingPlayer));
        prompt.Converter = p => $"{p.GetPlayerMarkup()} ({probs[p]:P2} likely to result in a win)";

        return AnsiConsole.Prompt(prompt);
    }

    public override GameRole GetInitialRoleClaim(Player player, GameState gameState)
    {
        SelectionPrompt<GameRole> prompt = new();
        prompt.Title("What role are you claiming you started as? (Actual: " + gameState.GetStartRole(player).AsMarkdown() + ")");
        prompt.AddChoices(gameState.Roles.Distinct());
        prompt.HighlightStyle(new Style(foreground: Color.White));
        prompt.Converter = r =>
        {
            float winProb = VotingHelper.GetAssumedStartRoleVictoryProbabilities(player, gameState, r);
                
            return $"{r.AsMarkdown()} ({RoleClaimVotingProbabilities.CalculateAverageBeliefProbability(gameState, player, r):P2} Likely to be believed, {winProb:P2} probable to win)";
        };
        
        return AnsiConsole.Prompt(prompt);
    }
}