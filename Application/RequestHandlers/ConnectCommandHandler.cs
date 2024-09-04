using ApplicationTemplate.Server.Commands;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class ConnectCommandHandler(IPlayerManager gameService, ILogger<ConnectCommandHandler> logger) : IRequestHandler<ConnectCommand>
    {
        private readonly IPlayerManager _gameService = gameService;
        private readonly ILogger<ConnectCommandHandler> _logger = logger;

        public async Task Handle(ConnectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"ConnectCommandHandler user id: {request.User.Id} ");

            await _gameService.AddPlayer(request.User);
        }
    }
}



