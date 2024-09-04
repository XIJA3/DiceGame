using Client.Services.IServices;
using Client.Services;
using Game;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Game.Services.IServices;
using Game.Services;
using MudBlazor.Services;
using MudBlazor;
using Blazored.LocalStorage;

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
        config.SnackbarConfiguration.PreventDuplicates = true;
        config.SnackbarConfiguration.NewestOnTop = true;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 3000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    });



    builder.Services.AddSingleton<IAuthService, AuthService>();
    builder.Services.AddSingleton<IGameHubService, GameHubService>();
    builder.Services.AddSingleton<IGameHubService, GameHubService>();
    builder.Services.AddScoped<ILeaderboardServiceClient, LeaderboardServiceClient>();
    
    builder.Services.AddSingleton<ILocalStorageManager, LocalStorageManager>();
    builder.Services.AddBlazoredLocalStorage();

    //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    await builder.Build().RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine($"#############{ex.Message}");
    throw;
}