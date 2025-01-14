using System.Diagnostics;

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
        await Page.GotoAsync("http://localhost:5049");
        await Expect(Page).ToHaveTitleAsync(new Regex("Wherewolf"));
    }

    [TestMethod]
    public async Task WelcomePageAllowsNavigationToConfigure()
    {
        await Page.GotoAsync("http://localhost:5049");

        // create a locator
        var getStarted = Page.Locator("text=Get Started");

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }
}