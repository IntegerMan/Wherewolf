namespace MattEland.Werewolf.BlazorTests;

public abstract class AppPageTest : PageTest
{
    public string BaseUrl => "http://localhost:5049/";
    
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
}