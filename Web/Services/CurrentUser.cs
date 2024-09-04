using DataTransferModels.Clients;
using DataTransferModels.DTO;
using DataTransferModels.ValueObjects;
using ApplicationTemplate.Server.Common.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using Web.Helpers;
using Web.Hubs;
using Web.Services.IServices;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Collections.Concurrent;

namespace Web.Services;


/// <summary>
/// Represents the current user connected to the SignalR hub. 
/// This class encapsulates user-related information, including the user's authentication status, connection details, 
/// client interface, and claims extracted from the JWT token. 
/// It is used to manage the user's session and interactions within the application.
/// </summary>
public class CurrentUser : IUser
{
    private readonly HubCallerContext _hubInvocationContext;
    private readonly HttpContext _httpContext;

    // Constructor to initialize properties using HubInvocationContext
    public CurrentUser(HubCallerContext hubInvocationContext, IApplicationClient client)
    {
        _hubInvocationContext = hubInvocationContext ?? throw new ArgumentNullException(nameof(hubInvocationContext));

        _httpContext = _hubInvocationContext.GetHttpContext()
                       ?? throw new InvalidOperationException("HttpContext is not available");

        Client = client;
    }

    public string HubConnectionId => _hubInvocationContext.ConnectionId;

    public bool IsAuthenticated => _httpContext.User.Identity?.IsAuthenticated ?? false;

    public DeviceInfo? DeviceInfo => _httpContext.Features.Get<IHttpConnectionFeature>()?.Detect(_httpContext);

    public IPAddress? IpAddress => _httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;

    public IApplicationClient Client { get; }

    public int Id => int.Parse(GetClaim(TokenClaim.UserId));

    public string UserName => GetClaim(TokenClaim.UserName);

    private string GetClaim(TokenClaim tokenClaim)
    {
        var claimValue = _httpContext.User?.FindFirst(tokenClaim.Name)?.Value;

        if (string.IsNullOrWhiteSpace(claimValue))
            throw new InvalidOperationException($"Claim {tokenClaim.Name} is not available");

        return claimValue;
    }
}
