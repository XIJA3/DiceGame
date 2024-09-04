using ApplicationTemplate.Server.Commands;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class DisconnectCommandHandler(
        IPlayerManager gameService, ILogger<DisconnectCommandHandler> logger) : IRequestHandler<DisconnectCommand>
    {
        private readonly IPlayerManager _gameService = gameService;
        private readonly ILogger<DisconnectCommandHandler> _logger = logger;

        public async Task Handle(DisconnectCommand request, CancellationToken cancellationToken)
        {
            // Todo: User request.exception

            await _gameService.RemovePlayer(request.User);

            _logger.LogInformation("User disconnected with Id:{Id}, UserAgent: {UserAgent}, IpAddress: {IpAddress}, Error: {Error}",
                request.User.Id, request.User.DeviceInfo?.UserAgent, request.User.IpAddress, request.exception?.Message);
        }
    }
}



