using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class JoinRoomCommandHandler(IMatchMakingService matchMakingService,  ILogger<JoinRoomCommandHandler> logger) : IRequestHandler<JoinRoomCommand>
    {
        private readonly IMatchMakingService _matchMakingService = matchMakingService;
        private readonly ILogger<JoinRoomCommandHandler> _logger = logger;

        public async Task Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} is attempting to join a room.", request.User.Id);
            await _matchMakingService.JoinRoom(request.User);
            _logger.LogInformation("User {UserId} has successfully joined a room.", request.User.Id);
        }
    }
}



