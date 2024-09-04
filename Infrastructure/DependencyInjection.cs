using ApplicationTemplate.Infrastructure.Data;
using ApplicationTemplate.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using NextLevel.DBContext;
using Microsoft.Extensions.DependencyInjection;
using ApplicationTemplate.Infrastructure.Repository;
using ApplicationTemplate.Server.Common.Interfaces;
using NextLevel.Security;
using ApplicationTemplate.Infrastructure.Services;
using ApplicationTemplate.Server.Services.IServices;
using StackExchange.Redis;
using Domain.IRepository;

namespace ApplicationTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<StartupService>();
        services.AddSingleton<EnumInitializationService>();

        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        
        // IDistributed Cache DI with Cluster configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { configuration.GetConnectionString("Redis") },
                AbortOnConnectFail = false,
                ConnectRetry = 3,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                ResponseTimeout = 5000
            };
            options.InstanceName = "GameTemplateRedis";
        });

        services.AddSingleton<ICacheService, CacheService>();
        //services.AddSingleton<IPlayerCacheService, PlayerCacheService>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("connectionString 333333333333333333333333333333333");

        Guard.Against.NullOrEmpty(connectionString, nameof(connectionString));

        var gameDbProviderString = configuration["DbProvider"];
        Guard.Against.NullOrEmpty(gameDbProviderString, nameof(gameDbProviderString));
        var gameDbProvider = (DbProviderType)Enum.Parse(typeof(DbProviderType), gameDbProviderString, true);

        var encryptedKey = CryptoService.Encrypt("pp3", "pp3");
        Guard.Against.NullOrEmpty(encryptedKey, nameof(encryptedKey));
        CryptoService.SetDefaultKey(encryptedKey);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseAny(connectionString, gameDbProvider));

        ApplicationDbContext.SetConnectionString(connectionString, "pp3", false, gameDbProvider);

        try
        {
            new ApplicationDbContext().Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddSingleton(TimeProvider.System);


        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();

        return services;
    }
}
