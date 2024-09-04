using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitializeDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialize = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initialize.Initialize();
    }
}

public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task Initialize()
    {
        try
        {
            //await _context.Database.MigrateAsync();

            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task ReCreateDatabase()
    {
        try
        {
            await _context.Database.EnsureDeletedAsync();

            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while recreating the database.");
            throw;
        }
    }
}
