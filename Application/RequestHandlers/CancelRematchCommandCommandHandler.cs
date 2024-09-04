using ApplicationTemplate.Server.Commands;
using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Server.RequestHandlers
{
    public class CancelRematchCommandCommandHandler(IMatchMakingService matchMakingService) : IRequestHandler<CancelRematchCommand>
    {
        private readonly IMatchMakingService _matchMakingService = matchMakingService;

        public async Task Handle(CancelRematchCommand request, CancellationToken cancellationToken)
        {
            await _matchMakingService.CancelRematch(request.User, request.SessionId);
        }
    }
}



