using DataTransferModels.ValueObjects;
using ApplicationTemplate.Server.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Web.Services.IServices;
using static Web.Helpers.JwtTokenManagerExtensions;
using Web.Helpers;

namespace Web.Services
{
    /// <summary>
    /// Service for handling JWT token generation, validation, and claims extraction. 
    /// This service provides methods to generate JWT tokens with custom claims, extract claims from tokens, 
    /// validate tokens, and retrieve the principal from tokens for authentication purposes.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT token with the specified claims. Adds a refresh token to the claims list.
        /// </summary>
        public string GenerateJwtToken(Dictionary<TokenClaim, string> tokenClaims)
        {
            var refreshToken = GenerateRefreshToken();

            var claims = tokenClaims.Select(pair => new Claim(pair.Key, pair.Value)).ToList();
            claims.Add(new Claim(TokenClaim.RefreshToken, refreshToken));

            var accessToken = GenerateToken(claims);
            return accessToken;
        }

        /// <summary>
        /// Retrieves and parses a specific claim from the given access token.
        /// </summary>
        public T GetTokenClaim<T>(string accessToken, TokenClaim tokenClaim, Func<string, T> parse)
        {
            var claimValue = GetTokenClaim(accessToken, tokenClaim);
            try
            {
                return parse(claimValue);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid {tokenClaim} in token", ex);
            }
        }

        /// <summary>
        /// Retrieves all claims from the provided JWT token as a ClaimsPrincipal.
        /// </summary>
        public ClaimsPrincipal GetAllTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadToken(token) as JwtSecurityToken;

            var claimsIdentity = new ClaimsIdentity(securityToken?.Claims, MyAuthenticationOptions.DefaultScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }

        /// <summary>
        /// Retrieves a specific claim from the access token.
        /// </summary>
        public string GetTokenClaim(string accessToken, TokenClaim tokenClaim)
        {
            var claim = JwtTokenManagerExtensions.GetTokenClaim(accessToken, tokenClaim)?.SingleOrDefault();

            if (string.IsNullOrWhiteSpace(claim))
                throw new ArgumentNullException("Token Claim Does Not Exist");

            return claim;
        }

        /// <summary>
        /// Extracts all relevant claims from the access token and returns them as a dictionary.
        /// </summary>
        public Dictionary<TokenClaim, string> GetAllTokenClaimsFromToken(string accessToken)
        {
            var userId = GetTokenClaim(accessToken, TokenClaim.UserId);
            var userName = GetTokenClaim(accessToken, TokenClaim.UserName);
            var refreshToken = GetTokenClaim(accessToken, TokenClaim.RefreshToken);
            var sessionId = GetTokenClaim(accessToken, TokenClaim.SessionId);
            var channel = GetTokenClaim(accessToken, TokenClaim.Channel);

            var tokenClaims = new Dictionary<TokenClaim, string>
            {
                { TokenClaim.UserId, userId },
                { TokenClaim.UserName, userName },
                { TokenClaim.SessionId, sessionId },
                { TokenClaim.Channel, channel },
                { TokenClaim.RefreshToken, refreshToken }
            };

            return tokenClaims;
        }

        /// <summary>
        /// Retrieves the ClaimsPrincipal from the given token, validating it against security parameters.
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            ValidateSecurityToken(securityToken);

            return principal;
        }

        /// <summary>
        /// Checks if the provided token is valid, returning true if it is, and false otherwise.
        /// </summary>
        public bool IsValidToken(string? token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return false;

                var principal = GetPrincipalFromToken(token);
                return principal != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
