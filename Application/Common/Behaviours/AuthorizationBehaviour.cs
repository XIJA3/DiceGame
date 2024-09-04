//using ApplicationTemplate.Server.Common.Exceptions;
//using ApplicationTemplate.Server.Common.Security;
//using System.Reflection;

//namespace ApplicationTemplate.Server.Common.Behaviours;


///*
//    I Commented this Hebavior because it's used in case there are any
//    roles, policy or any other way to authorize user
//    currently there is no need to use any of it.
// */

//public class AuthorizationBehaviour<TRequest, TResponse>(
//    IUser user,
//    IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
//{
//    private readonly IUser _user = user;
//    private readonly IIdentityService _identityService = identityService;

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

//        if (authorizeAttributes.Any())
//        {
//            // Must be authenticated user
//            if (_user.Id == null)
//            {
//                throw new UnauthorizedAccessException();
//            }

//            // Role-based authorization
//            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

//            if (authorizeAttributesWithRoles.Any())
//            {
//                var authorized = false;

//                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
//                {
//                    foreach (var role in roles)
//                    {
//                        var isInRole = await _identityService.IsInRoleAsync(_user.Id, role.Trim());
//                        if (isInRole)
//                        {
//                            authorized = true;
//                            break;
//                        }
//                    }
//                }

//                // Must be a member of at least one role in roles
//                if (!authorized)
//                {
//                    throw new ForbiddenAccessException();
//                }
//            }

//            // Policy-based authorization
//            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
//            if (authorizeAttributesWithPolicies.Any())
//            {
//                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
//                {
//                    var authorized = await _identityService.AuthorizeAsync(_user.Id, policy);

//                    if (!authorized)
//                    {
//                        throw new ForbiddenAccessException();
//                    }
//                }
//            }
//        }

//        // User is authorized / authorization not required
//        return await next();
//    }
//}
