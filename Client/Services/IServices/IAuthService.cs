
// Interface for dependency injection
public interface IAuthService
{
    string? AccessToken { get; }
    Task InitializeAsync();
    Task LoginAsync(string userName, string password);
    Task RemoveSessionAsync(HttpRequestMessage? request = null);
    Task ReadSessionAsync(HttpRequestMessage? request = null);
    string? GetUserName();
    bool IsAuthorized();
}
