using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //_timer.Start();

        var response = await next();

        //_timer.Stop();

        //var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        //if (elapsedMilliseconds > 500)
        //{
        //    var requestName = typeof(TRequest).Name;

        //    var userId = _user.Id;
        //    var userName = _user.UserName;

        //    _logger.LogWarning("ApplicationTemplate Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
        //        requestName, elapsedMilliseconds, userId, userName, request);
        //}

        return response;
    }
}
