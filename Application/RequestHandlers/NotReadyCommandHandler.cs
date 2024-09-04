using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class NotReadyCommandHandler(IMatchMakingService matchMakingService, ILogger<NotReadyCommandHandler> logger) : IRequestHandler<NotReadyCommand>
    {
        private readonly IMatchMakingService _matchMakingService = matchMakingService;
        private readonly ILogger<NotReadyCommandHandler> _logger = logger;

        public async Task Handle(NotReadyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} is not ready.", request.User.Id);
            await _matchMakingService.NotReady(request.User);
            _logger.LogInformation("User {UserId} is now marked as not ready.", request.User.Id);
        }
    }
}
