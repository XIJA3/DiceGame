using ApplicationTemplate.Infrastructure.Data;
using ApplicationTemplate.Server.Common.Interfaces;
using Azure.Identity;
using DataTransferModels.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Web.Helpers;
using Web.Hubs;
using Web.Middlewares;
using Web.Services;
using Web.Services.IServices;

namespace Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddHttpContextAccessor();

            services.AddSignalR();
            services.AddRazorPages();

            //services.AddScoped<IUser, CurrentUser>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();

            services.AddScoped<IMediatorService, MediatorService>();

            var clientUri = configuration["ClientUrl"]
                ?? throw new Exception("ClientUrl is null");

            // todo move this in settings
            services.AddCors(options =>
            {
                options.AddPolicy("SignalRHUB", builder => builder
                    .WithOrigins(clientUri)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });


            // Cache Configuration
            services.AddDistributedMemoryCache();

            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? throw new ArgumentNullException(nameof(configuration));

            services.AddHealthChecks()
                .AddRedis(redisConnectionString, name: "Redis", failureStatus: HealthStatus.Unhealthy);


            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            services.AddScoped<CustomAuthentication>();
            services.AddScoped<CustomTokenHandler>();

            services.AddAuthentication(MyAuthenticationOptions.DefaultScheme)
                .AddScheme<MyAuthenticationOptions, CustomTokenHandler>(
                    MyAuthenticationOptions.DefaultScheme,
                    options => JwtTokenManagerExtensions.GetValidationParameters());

            services.AddAuthorization();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.AddFilter<HubFilter>();
            });

            //services.AddHealthChecks()
            //    .AddDbContextCheck<ApplicationDbContext>();

            services.AddExceptionHandler<CustomExceptionHandler>();

            services.AddRazorPages();

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            services.AddEndpointsApiExplorer();

            services.AddOpenApiDocument((configure, sp) =>
            {
                configure.Title = "GameTemplate API";
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Example API",
                    Version = "v1",
                    Description = "An example of an ASP.NET Core Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Email = "example@example.com",
                        Url = new Uri("https://example.com/contact"),
                    },
                });

                options.AddSecurityDefinition(MyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = MyAuthenticationOptions.DefaultScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = MyAuthenticationOptions.DefaultScheme
                        },
                        Scheme = "Oauth2",
                        Name = MyAuthenticationOptions.DefaultScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
            });

            return services;
        }
    }
}
