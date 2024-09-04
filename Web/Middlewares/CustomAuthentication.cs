using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using ApplicationTemplate.Server.Common.Interfaces;
using DataTransferModels.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using static Web.Helpers.MiddlewareExtensions;
using static Web.Helpers.HttpContextExtensions;
using static Web.Helpers.ClaimsPrincipalExtensions;
using Microsoft.AspNetCore.Http;
using Domain.Models;
using Web.Helpers;
using Web.Services;
using Web.Services.IServices;
using Web.Hubs;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Web.Middlewares
{
    public class CustomAuthentication(IJwtTokenService jwtTokenManager, IUserAuthenticationService authenticationService,
        ILogger<CustomAuthentication> logger)
    {
        private readonly IUserAuthenticationService _authenticationService = authenticationService;
        private readonly IJwtTokenService _jwtTokenManager = jwtTokenManager;
        private readonly ILogger<CustomAuthentication> _logger  = logger;

        public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, MethodInfo? hubMethodInfo = null)
        {
            try
            {
                // Authorization Requirement Check
                if (!await RequiresAuthorization(context, hubMethodInfo))
                    return GetAnonymousAccessTicket();

                // Validating Token
                var accessToken = context.GetTokenFromContext();

                if (string.IsNullOrWhiteSpace(accessToken))
                    return AuthenticateResult.Fail("Invalid Token.");

                // If token is not cached or expired, validate and refresh if necessary
                if (!_jwtTokenManager.IsValidToken(accessToken))
                {
                    var response = await _authenticationService.TryRefreshTokenAsync(accessToken);

                    accessToken = response.AccessToken;

                    if (!response.IsSuccessful || !_jwtTokenManager.IsValidToken(accessToken) || string.IsNullOrWhiteSpace(accessToken))
                        return AuthenticateResult.Fail("Could not refresh token.");
                    else
                        context.WriteHttpOnlyCookie(Cookie.AccessToken, accessToken);
                }

                // Get principal from token
                var principal = _jwtTokenManager.GetAllTokenClaims(accessToken);

                // Set the authenticated user in HttpContext
                context.User = principal;
 
                return AuthenticateResult.Success(new AuthenticationTicket(principal, MyAuthenticationOptions.DefaultScheme));
            }
            catch (Exception ex)
            {
                context.DeleteCookie(Cookie.AccessToken);

                _logger.LogInformation($"Unauthorized {ex.Message}");

                return AuthenticateResult.Fail("Unauthorized: " + ex.Message);
            }
        }

        public class PrincipalCacheData
        {
            public SerializableClaimsPrincipal Principal { get; set; } = new SerializableClaimsPrincipal();
            public DateTime ExpirationDate { get; set; }
        }
    }
}
