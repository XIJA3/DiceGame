using Client;
using Client.Services;
using Client.Services.IServices;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");


    builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
    builder.Services.AddSingleton<IAuthService, AuthService>();
    builder.Services.AddSingleton<IHubService, HubService>();


    builder.Services.AddMudServices();

    await builder.Build().RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);

    throw;
}