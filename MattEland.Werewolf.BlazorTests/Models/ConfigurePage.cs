using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests.Models;

public class ConfigurePage(IPage page) : WerewolfPageBase(page)
{
    private readonly IPage _page = page;
    
    public ILocator PageHeader => _page.GetByTestId("PageHeader");
    public ILocator PageSubtitle => _page.GetByTestId("PageSubtitle");
    public ILocator PlayGameButton => _page.GetByTestId("PlayGameButton");

    public override string Url => $"{BaseUrl}Configure";
}