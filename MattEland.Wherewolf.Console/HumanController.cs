using MattEland.Wherewolf.Controllers;
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
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select a player to rob")
            .AddChoices(otherPlayerNames));
    }
}