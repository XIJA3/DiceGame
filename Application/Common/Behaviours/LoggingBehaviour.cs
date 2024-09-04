using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) :
    IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger = logger;

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        //var requestName = typeof(TRequest).Name;

        //var userId = _user.Id;
        //var userName = _user.UserName;

        //_logger.LogInformation("ApplicationTemplate Request: {Name} {@UserId} {@UserName} {@Request}",
        //    requestName, userId, userName, request);
    }
}
