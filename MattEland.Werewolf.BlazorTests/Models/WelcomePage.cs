using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests.Models;

public class WelcomePage(IPage page) : WerewolfPageBase(page)
{
    public ILocator PageHeader => Page.GetByTestId("PageHeader");
    public ILocator PageSubtitle => Page.GetByTestId("PageSubtitle");
    public ILocator PlayGameButton => Page.GetByTestId("PlayGameButton");

    public override string Url => BaseUrl;
}