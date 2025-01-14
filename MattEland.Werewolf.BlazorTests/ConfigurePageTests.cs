using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests;

public class ConfigurePageTests : AppPageTest
{
    [TestMethod]
    public async Task ConfigurePageHasCorrectContent()
    {
        await Page.GotoAsync($"{BaseUrl}Configure");
        await Expect(Page).ToHaveTitleAsync(new Regex("Configure Game"));

        await Expect(Page.GetByTestId("PageHeader")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading)).ToContainTextAsync("Configure Game");
        
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Play Game" })).ToBeVisibleAsync();
    }
}