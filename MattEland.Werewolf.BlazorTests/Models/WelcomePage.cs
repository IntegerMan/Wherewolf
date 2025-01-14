using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests.Models;

public class WelcomePage(IPage page) : WerewolfPageBase(page)
{
    public ILocator PageHeader => page.GetByTestId("PageHeader");
    public ILocator PageSubtitle => page.GetByTestId("PageSubtitle");
    public ILocator PlayGameButton => page.GetByTestId("PlayGameButton");

    public override string Url => BaseUrl;
}