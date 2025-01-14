using System.Diagnostics;
using Microsoft.Playwright;
using MudBlazor;

namespace MattEland.Werewolf.BlazorTests;

[TestClass]
public class WelcomePageTests : PageTest
{
    /*
    private Process? _blazorServerProcess;

    [TestInitialize]
    public async Task Setup()
    {
        _blazorServerProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project ../MattEland.Wherewolf.BlazorFrontEnd",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        _blazorServerProcess.Start();
        await Task.Delay(5000); // Wait for the server to start
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_blazorServerProcess != null && !_blazorServerProcess.HasExited)
        {
            _blazorServerProcess.Kill();
        }
    }
    */
    
    [TestMethod]
    public async Task WelcomePageHasCorrectContent()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page).ToHaveTitleAsync(new Regex("Wherewolf"));

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Wherewolf?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading)).ToContainTextAsync("Wherewolf?");
    }

    [TestMethod]
    public async Task WelcomePageAllowsNavigationToConfigure()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Play Game" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{BaseUrl}Configure");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Configure Game" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading)).ToContainTextAsync("Configure Game");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Play Game" })).ToBeVisibleAsync();

    }

    public string BaseUrl => "http://localhost:5049/";
}