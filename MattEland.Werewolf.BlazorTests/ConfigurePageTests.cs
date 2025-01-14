using MattEland.Werewolf.BlazorTests.Models;
using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests;

[TestClass]
public class ConfigurePageTests : PageTest
{
    private ConfigurePage _pageModel = null!;

    [TestInitialize]
    public async Task TestInitializeAsync()
    {
        _pageModel = new ConfigurePage(Page);
        await _pageModel.LoadAsync();
    }
    
    [TestMethod]
    public async Task ConfigurePageHasCorrectContent()
    {
        await Expect(Page).ToHaveTitleAsync(new Regex("Configure Game"));

        await Expect(_pageModel.PageHeader).ToBeVisibleAsync();
        await Expect(_pageModel.PageSubtitle).ToBeVisibleAsync();
        await Expect(_pageModel.PlayGameButton).ToBeVisibleAsync();
        
        await Expect(_pageModel.PageHeader).ToContainTextAsync("Configure Game");
        await Expect(_pageModel.PlayGameButton).ToContainTextAsync("Play Game");
    }
}