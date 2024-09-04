using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class RematchCommandHandler(IMatchMakingService matchMakingService) : IRequestHandler<RematchCommand>
    {
        private readonly IMatchMakingService _matchMakingService = matchMakingService;

        public async Task Handle(RematchCommand request, CancellationToken cancellationToken)
        {
            await _matchMakingService.Rematch(request.User, request.SessionId);
        }
    }
}



