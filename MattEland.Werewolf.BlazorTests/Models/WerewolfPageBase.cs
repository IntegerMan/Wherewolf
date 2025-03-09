using Microsoft.Playwright;

namespace MattEland.Werewolf.BlazorTests.Models;

public abstract class WerewolfPageBase(IPage page)
{
    public static string BaseUrl => "http://localhost:5049/";
    
    public abstract string Url { get; }    
    
    public async Task LoadAsync() 
        => await page.GotoAsync(Url);

    protected IPage Page => page;
}