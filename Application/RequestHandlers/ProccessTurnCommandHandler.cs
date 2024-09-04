using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class ProccessTurnCommandHandler(ISessionService sessionService, ILogger<ProccessTurnCommandHandler> logger) : IRequestHandler<ProccessTurnCommand>
    {
        private readonly ISessionService _sessionService = sessionService;
        private readonly ILogger<ProccessTurnCommandHandler> _logger = logger;

        public async Task Handle(ProccessTurnCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} is not ready.", request.User.Id);
            await _sessionService.ProcessTurnAsync(request.SessionId, request.SessionPlayId, request.User, request.DiceSetType);
            _logger.LogInformation("User {UserId} is now marked as not ready.", request.User.Id);
        }
    }
}



