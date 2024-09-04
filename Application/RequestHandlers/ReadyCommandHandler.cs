using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class ReadyCommandHandler(IMatchMakingService matchMakingService, ILogger<ReadyCommandHandler> logger) : IRequestHandler<ReadyCommand>
    {
        private readonly IMatchMakingService _matchMakingService = matchMakingService;
        private readonly ILogger<ReadyCommandHandler> _logger = logger;

        public async Task Handle(ReadyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} is ready.", request.User.Id);
            await _matchMakingService.Ready(request.User);
            _logger.LogInformation("User {UserId} is now marked as ready.", request.User.Id);
        }
    }
}



