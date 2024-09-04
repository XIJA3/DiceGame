using ApplicationTemplate.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Web.Helper;
using Web.Helpers;
using Web.Hubs;

using Web;
using ApplicationTemplate.Server;
using Web.Middlewares;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    GlobalConfiguration.Configure(builder.Configuration);

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebServices(builder.Configuration);

    ConfigurationHelper.ConfigureSerilog(builder);

    var app = builder.Build();

    //if (app.Environment.IsDevelopment())
    //{
    //    await app.InitializeDatabase();
    //}
    //else
    //{
    //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //    app.UseHsts();
    //}

    app.UseDeveloperExceptionPage();

    app.UseHealthChecks("/health");
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    
    var forwardedHeadersOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    };

    app.UseForwardedHeaders(forwardedHeadersOptions);

    app.UseAuthentication(); // Ensure authentication middleware is added
    app.UseAuthorization();  // Ensure authorization middleware is added

    app.UseSwaggerUi(settings =>
    {
        settings.Path = "/api";
        settings.DocumentPath = "/api/specification.json";
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");

    app.MapRazorPages();

    app.MapFallbackToFile("index.html");

    app.UseExceptionHandler(options => { });

#if (USEAPIONLY)
app.Map("/", () => Results.Redirect("/api"));
#endif

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseCors("SignalRHUB");
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapHub<ApplicationHub>("/gameHub");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Error3333333333333" + ex.Message + "3333333InnerError" + ex.InnerException?.Message);

    throw;
}