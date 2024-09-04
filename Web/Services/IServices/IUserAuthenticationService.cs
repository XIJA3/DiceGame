using DataTransferModels.Requests;
using DataTransferModels.Responses;
using ApplicationTemplate.Server.Common.Interfaces;

namespace Web.Services.IServices
{
    public interface IUserAuthenticationService
    {
        Task<UserSession> LogInAsync(LoginRequest request);
        Task<TryRefreshTokenResponse> TryRefreshTokenAsync(string AccessToken);
    }
}
