using DataTransferModels.ValueObjects;
using System.Security.Claims;

namespace Web.Services.IServices
{
    public interface IJwtTokenService
    {
        T GetTokenClaim<T>(string accessToken, TokenClaim tokenClaim, Func<string, T> parse);
        string GetTokenClaim(string accessToken, TokenClaim tokenClaim);
        Dictionary<TokenClaim, string> GetAllTokenClaimsFromToken(string accessToken);
        string GenerateJwtToken(Dictionary<TokenClaim, string> tokenClaims);
        ClaimsPrincipal GetAllTokenClaims(string accessToken);
        ClaimsPrincipal GetPrincipalFromToken(string accessToken);
        bool IsValidToken(string? accessToken);
    }
}