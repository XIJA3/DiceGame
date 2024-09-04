using DataTransferModels.Requests;
using DataTransferModels.Responses;
using DataTransferModels.ValueObjects;
using Domain.IRepository;
using Domain.Models;
using ApplicationTemplate.Server.Common.Interfaces;
using Web.Services.IServices;

namespace Web.Services
{
    /// <summary>
    /// Service handling user authentication, including login and token refresh operations.
    /// </summary>
    public class UserAuthenticationService(
        IJwtTokenService jwtTokenManager,
        IPlayerRepository playerRepository,
        IRefreshTokenRepository refreshTokenRepository) : IUserAuthenticationService
    {
        private readonly IJwtTokenService _jwtTokenService = jwtTokenManager;
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        /// <summary>
        /// Logs in a user, creating a new user if they don't exist, and returns a user session with an access token.
        /// </summary>
        public async Task<UserSession> LogInAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserName))
                    throw new UnauthorizedAccessException("UserName is not valid");

                var user = await _playerRepository.GetPlayerByName(request.UserName);
                user ??= await _playerRepository.CreateAsync(request.UserName);

                var tokenClaims = new Dictionary<TokenClaim, string>
                {
                    { TokenClaim.UserId, user.Id.ToString() },
                    { TokenClaim.UserName, user.UserName }
                };

                var accessToken = _jwtTokenService.GenerateJwtToken(tokenClaims);
                return new UserSession(accessToken, user.UserName);
            }
            catch (Exception)
            {
                // TODO: Log error
                throw new UnauthorizedAccessException("Cannot log in.");
            }
        }

        /// <summary>
        /// Attempts to refresh the user's access token using a valid refresh token.
        /// </summary>
        public async Task<TryRefreshTokenResponse> TryRefreshTokenAsync(string AccessToken)
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
                return new TryRefreshTokenResponse(false, string.Empty);

            var tokenClaims = _jwtTokenService.GetAllTokenClaimsFromToken(AccessToken);

            var userId = tokenClaims[TokenClaim.UserId];
            var userName = tokenClaims[TokenClaim.UserName];
            var refreshToken = tokenClaims[TokenClaim.RefreshToken];

            var isValidRefreshToken = _refreshTokenRepository.IsValidRefreshToken(userName, refreshToken);
            if (!isValidRefreshToken)
                return new TryRefreshTokenResponse(false, string.Empty);

            var user = await _playerRepository.GetPlayerById(Convert.ToInt64(userId));
            if (user is null)
                return new TryRefreshTokenResponse(false, string.Empty);

            var newAccessToken = _jwtTokenService.GenerateJwtToken(tokenClaims);
            if (string.IsNullOrEmpty(newAccessToken))
                return new TryRefreshTokenResponse(false, string.Empty);

            var newRefreshToken = _jwtTokenService.GetTokenClaim(newAccessToken, TokenClaim.RefreshToken);

            var userRefreshToken = new UserRefreshToken
            {
                RefreshToken = newRefreshToken,
                UserName = userName
            };

            _refreshTokenRepository.DeleteUserRefreshTokens(userName, newRefreshToken);
            _refreshTokenRepository.AddUserRefreshTokens(userRefreshToken);

            return new TryRefreshTokenResponse(true, newAccessToken);
        }
    }
}
