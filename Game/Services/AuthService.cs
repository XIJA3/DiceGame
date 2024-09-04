using Client.Services.IServices;
using DataTransferModels.Requests;
using DataTransferModels.Responses;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    /// <summary>
    /// Provides authentication services including login, logout, and session management.
    /// </summary>
    public class AuthService(ILocalStorageManager localStorageService, IConfiguration configuration) : IAuthService
    {
        // Local storage manager for handling session data in local storage
        private readonly ILocalStorageManager _localStorageService = localStorageService;
        // Configuration for retrieving application settings
        private readonly IConfiguration _configuration = configuration;

        // Access token retrieved from the session storage
        public string? AccessToken => ReadSessionFromStorageAsync().GetAwaiter().GetResult()?.AccessToken;

        /// <summary>
        /// Asynchronously retrieves the access token from session storage.
        /// </summary>
        /// <returns>The access token if available; otherwise, null.</returns>
        public async Task<string?> GetAccessTokenAsync()
        {
            return (await ReadSessionFromStorageAsync())?.AccessToken;
        }

        // HttpClient instance used for making HTTP requests
        private HttpClient _httpClient;

        /// <summary>
        /// Gets the HttpClient instance configured with the base URL from the configuration.
        /// </summary>
        internal HttpClient HttpClient
        {
            get
            {
                var baseUrl = _configuration?["ServerBaseUri"]
                    ?? throw new InvalidOperationException("ServerBaseUri is not configured.");

                return _httpClient ??= new HttpClient()
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromMinutes(5)
                };
            }
        }

        /// <summary>
        /// Asynchronously logs in the user with the provided user name.
        /// </summary>
        /// <param name="userName">The user name for login.</param>
        /// <returns>True if login was successful; otherwise, false.</returns>
        public async Task<bool> LoginAsync(string userName)
        {
            try
            {
                var url = "/api/Authorization/LogIn";
                var loginRequest = new LoginRequest(userName);

                // Send login request to the server
                var response = await HttpClient.PostAsJsonAsync(url, loginRequest);
                response.EnsureSuccessStatusCode();

                // Read the user session from the response
                var userSession = await response.Content.ReadFromJsonAsync<UserSession>();

                if (userSession is null)
                {
                    return false;
                }

                // Save the session data
                await SaveSessionAsync(userSession);

                return true;
            }
            catch (HttpRequestException)
            {
                // Handle HTTP request exceptions
                return false;
            }
            catch (Exception)
            {
                // Handle other exceptions
                return false;
            }
        }

        /// <summary>
        /// Asynchronously logs out the user by removing session data.
        /// </summary>
        public async Task LogoutAsync()
        {
            await RemoveSessionAsync();
        }

        /// <summary>
        /// Removes session data and authorization headers.
        /// </summary>
        /// <param name="request">Optional HttpRequestMessage to remove headers from.</param>
        public async Task RemoveSessionAsync(HttpRequestMessage? request = null)
        {
            if (request == null)
                RemoveAuthorizationHeader();
            else
                RemoveAuthorizationHeader(request);

            await RemoveCookieAsync();
        }

        /// <summary>
        /// Reads the user session from storage and updates the current session.
        /// </summary>
        /// <param name="request">Optional HttpRequestMessage to update headers for.</param>
        public async Task ReadSessionAsync(HttpRequestMessage? request = null)
        {
            var session = await ReadSessionFromStorageAsync();

            if (session == null)
                await RemoveSessionAsync(request);
            else
                await SaveSessionAsync(session);
        }

        /// <summary>
        /// Removes the Authorization header from the request and default headers.
        /// </summary>
        /// <param name="request">Optional HttpRequestMessage to remove headers from.</param>
        private void RemoveAuthorizationHeader(HttpRequestMessage? request = null)
        {
            if (request is not null)
            {
                request.Headers.Remove("Authorization");
                request.Headers.Accept.Clear();
            }

            HttpClient.DefaultRequestHeaders.Remove("Authorization");
            HttpClient.DefaultRequestHeaders.Accept.Clear();
        }

        /// <summary>
        /// Updates the Authorization header with the provided token.
        /// </summary>
        /// <param name="token">The token to set in the Authorization header.</param>
        /// <param name="request">Optional HttpRequestMessage to update headers for.</param>
        internal void UpdateAuthorizationHeader(string token, HttpRequestMessage? request = null)
        {
            if (request is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Retrieves the Authorization header value.
        /// </summary>
        /// <returns>The token if available; otherwise, null.</returns>
        internal string? GetAuthorizationHeader()
        {
            var authorizationHeader = HttpClient.DefaultRequestHeaders.Authorization;

            if (authorizationHeader != null)
            {
                var tokenParts = authorizationHeader.ToString().Split(" ");

                if (tokenParts is not null && tokenParts.Length == 2 && tokenParts[0] == "Bearer")
                    return tokenParts[1];

                return null;
            }

            return null;
        }

        /// <summary>
        /// Asynchronously retrieves the user name from the session storage.
        /// </summary>
        /// <returns>The user name if available; otherwise, null.</returns>
        public async Task<string?> GetUserName()
        {
            return (await ReadSessionFromStorageAsync())?.UserName;
        }

        /// <summary>
        /// Asynchronously checks if the user is authorized based on session storage.
        /// </summary>
        /// <returns>True if the user is authorized; otherwise, false.</returns>
        public async Task<bool> IsAuthorized()
        {
            return (await ReadSessionFromStorageAsync()) is not null;
        }

        /// <summary>
        /// Saves the user session to storage and updates the authorization header.
        /// </summary>
        /// <param name="session">The user session to save.</param>
        private async Task SaveSessionAsync(UserSession session)
        {
            await RemoveCookieAsync();
            await SaveCookieAsync(session);

            UpdateAuthorizationHeader(session.AccessToken);
        }

        /// <summary>
        /// Removes the session data from local storage.
        /// </summary>
        private async Task RemoveCookieAsync()
        {
            await _localStorageService.RemoveItem("Session");
        }

        /// <summary>
        /// Saves the user session to local storage.
        /// </summary>
        /// <param name="session">The user session to save.</param>
        private async Task SaveCookieAsync(UserSession session)
        {
            await _localStorageService.SetItem("Session", session);
        }

        /// <summary>
        /// Reads the user session from local storage.
        /// </summary>
        /// <returns>The user session if available; otherwise, null.</returns>
        private async Task<UserSession?> ReadSessionFromStorageAsync()
        {
            try
            {
                var session = await _localStorageService.GetItem<UserSession>("Session");
                return session ?? default;
            }
            catch (Exception)
            {
                // Handle exceptions during session retrieval
                return default;
            }
        }
    }
}
