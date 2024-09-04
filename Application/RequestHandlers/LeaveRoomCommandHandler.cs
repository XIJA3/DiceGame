using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class LeaveRoomCommandHandler(IMatchMakingService matchMakingService,  ILogger<LeaveRoomCommandHandler> logger) : IRequestHandler<LeaveRoomCommand>
    {
        private readonly IMatchMakingService _matchMakingService = matchMakingService;
        private readonly ILogger<LeaveRoomCommandHandler> _logger = logger;

        public async Task Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} is attempting to leave the room.", request.User);
            await _matchMakingService.LeaveRoom(request.User, request.SessionId);
            _logger.LogInformation("User {UserId} has successfully left the room.", request.User);
        }
    }
}



