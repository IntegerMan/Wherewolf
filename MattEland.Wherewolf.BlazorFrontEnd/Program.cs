using ApexCharts;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MattEland.Wherewolf.BlazorFrontEnd;
using MattEland.Wherewolf.BlazorFrontEnd.Client;
using MattEland.Wherewolf.Services;
using MattEland.Wherewolf.Services.Repositories;
using MudBlazor.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApexCharts();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<PlayerWebController>();
builder.Services.AddSingleton<IGameStateRepository, InMemoryGameStateRepository>();

await builder.Build().RunAsync();