using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TowerDefenseBlazor;
using TowerDefenseBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<HttpClient>(sp =>
    new HttpClient { BaseAddress = new Uri("https://localhost:7268") }); // URL API

// Регистрации сервиса лидерборда
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

await builder.Build().RunAsync();