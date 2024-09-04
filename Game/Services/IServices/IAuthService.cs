
// Interface for dependency injection
public interface IAuthService
{
    string? AccessToken { get; }
    Task<string?> GetAccessTokenAsync();
    Task<bool> LoginAsync(string userName);
    Task LogoutAsync();
    Task RemoveSessionAsync(HttpRequestMessage? request = null);
    Task ReadSessionAsync(HttpRequestMessage? request = null);
     Task<string?> GetUserName();
    Task<bool> IsAuthorized();
}
