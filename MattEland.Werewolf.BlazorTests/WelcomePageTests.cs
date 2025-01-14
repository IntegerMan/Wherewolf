
using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests;

[TestClass]
public class WelcomePageTests : AppPageTest
{
    [TestMethod]
    public async Task WelcomePageHasCorrectContent()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page).ToHaveTitleAsync(new Regex("Wherewolf"));

        ILocator headerElement = Page.GetByTestId("PageHeader");
        await Expect(headerElement).ToBeVisibleAsync();
        await Expect(headerElement).ToContainTextAsync("Wherewolf?");
        
        ILocator playButton = Page.GetByTestId("PlayGameButton");
        await Expect(playButton).ToBeVisibleAsync();
        await Expect(playButton).ToHaveTextAsync("Play Game");
    }

    [TestMethod]
    public async Task WelcomePageAllowsNavigationToConfigure()
    {
        await Page.GotoAsync(BaseUrl);
        
        ILocator playButton = Page.GetByTestId("PlayGameButton");
        await playButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{BaseUrl}Configure");
    }
}