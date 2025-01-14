using MattEland.Werewolf.BlazorTests.Models;

namespace MattEland.Werewolf.BlazorTests.Pages;

[TestClass]
public class WelcomePageTests : PageTest
{
    private WelcomePage _pageModel = null!;

    [TestInitialize]
    public async Task TestInitializeAsync()
    {
        _pageModel = new WelcomePage(Page);
        await _pageModel.LoadAsync();
    }

    [TestMethod]
    public async Task WelcomePageHasCorrectContent()
    {
        await Expect(Page).ToHaveTitleAsync(new Regex("Wherewolf"));

        await Expect(_pageModel.PageHeader).ToBeVisibleAsync();
        await Expect(_pageModel.PageSubtitle).ToBeVisibleAsync();
        await Expect(_pageModel.PlayGameButton).ToBeVisibleAsync();
        
        await Expect(_pageModel.PageHeader).ToContainTextAsync("Wherewolf?");
        await Expect(_pageModel.PlayGameButton).ToHaveTextAsync("Play Game");
    }

    [TestMethod]
    public async Task WelcomePageAllowsNavigationToConfigure()
    {
        await _pageModel.PlayGameButton.ClickAsync();
        
        ConfigurePage configurePage = new(Page);
        await Expect(Page).ToHaveURLAsync(configurePage.Url);
    }
}