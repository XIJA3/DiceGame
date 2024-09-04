using System.Reflection;
using ApplicationTemplate.Server.Common.Behaviours;
using ApplicationTemplate.Server.Common.Interfaces;
using ApplicationTemplate.Server.Services;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IPlayerManager, PlayerManager>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddSingleton<IDiceService, DiceService>();
        services.AddSingleton<IMatchMakingService, MatchMakingService>();
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<IScopedServiceExecutor, ScopedServiceExecutor>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(GameValidationBehaviour<,>));
            // Comment Reason inside AuthorizationBehaviour
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        });

        return services;
    }
}
