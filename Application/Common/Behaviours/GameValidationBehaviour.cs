using ApplicationTemplate.Server.Common.Security;
using System.Reflection;

namespace ApplicationTemplate.Server.Common.Behaviours;

public class GameValidationBehaviour<TRequest, TResponse>(
    IPlayerManager gameService) : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    private readonly IPlayerManager _gameService = gameService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Todo: Move this To Authentication Behavior

        //var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        //if (authorizeAttributes.Any())
        //{
        //    var gamePlayer = await _gameService.TryGetPlayer(_user);

        //    if (gamePlayer is not null && gamePlayer.IsBanned)
        //    {
        //        _ = _user.Client.YouAreBanned();
        //        throw new Exception("YouAreBanned");
        //    }
        //}

        return await next();
    }
}
