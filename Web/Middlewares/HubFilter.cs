using ApplicationTemplate.Server.Common.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Web.Services;
using Web.Services.IServices;

namespace Web.Middlewares;

public class HubFilter(CustomAuthentication customAuthentication,
    IOptionsMonitor<MyAuthenticationOptions> options) : IHubFilter
{
    private readonly CustomAuthentication _customAuthentication = customAuthentication;
    private readonly MyAuthenticationOptions _options = options.CurrentValue;

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var context = invocationContext.Context.GetHttpContext()!;

        var method = invocationContext.HubMethod;

        try
        {
            var result = await _customAuthentication.AuthenticateAsync(context, method);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Can Not Authenticate User");
            }

            return await next(invocationContext);
        }
        catch (UnauthorizedAccessException ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    //Todo: Figure out what we need to log
    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var httpContext = context.Context.GetHttpContext();

        if (httpContext != null)
        {
            var result = await _customAuthentication.AuthenticateAsync(httpContext);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Can Not Authenticate User");
        }

        await next(context);
    }

    //Todo: Figure out what we need to log
    public async Task OnDisconnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next, Exception exception)
    {
        await next(context);
    }
}