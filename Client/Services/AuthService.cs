using Client.Services.IServices;
using DataTransferModels.Responses;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AuthService(ILocalStorageService localStorageService, IConfiguration configuration) : IAuthService
    {
        private UserSession? _userPermission;
        private readonly ILocalStorageService _localStorageService = localStorageService;
        private readonly IConfiguration _configuration = configuration;

        public string? AccessToken => _userPermission?.AccessToken;

        private HttpClient _httpClient;
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

        public async Task InitializeAsync()
        {
            await ReadSessionAsync();
        }

        public async Task LoginAsync(string userName, string password)
        {
            var url = "https://yourapiurl.com/api/Authorization/LogIn";
            var loginRequest = new { Username = userName, Password = password };

            // Send a POST request with the LoginRequest object
            var response = await _httpClient.PostAsJsonAsync(url, loginRequest);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Deserialize the response content into a UserSession object
            var userSession = await response.Content.ReadFromJsonAsync<UserSession>();

            await SaveSessionAsync(userSession);
        }

        public async Task RemoveSessionAsync(HttpRequestMessage? request = null)
        {
            _userPermission = null;

            if (request == null)
                RemoveAuthorizationHeader();
            else
                RemoveAuthorizationHeader(request);

            await RemoveCookieAsync();
        }

        public async Task ReadSessionAsync(HttpRequestMessage? request = null)
        {
            var session = _userPermission ?? await ReadSessionFromStorageAsync();

            if (session == null)
            {
                await RemoveSessionAsync(request);
            }
            else
            {
                var accessToken = GetAuthorizationHeader();

                if (!string.IsNullOrWhiteSpace(accessToken) && session.AccessToken != accessToken)
                    _userPermission.AccessToken = accessToken;

                await SaveSessionAsync(session);
            }
        }

        private  void RemoveAuthorizationHeader(HttpRequestMessage? request = null)
        {
            if (request is not null)
            {
                request.Headers.Remove("Authorization");
                request.Headers.Accept.Clear();
            }

            HttpClient.DefaultRequestHeaders.Remove("Authorization");
            HttpClient.DefaultRequestHeaders.Accept.Clear();

        }

        internal void UpdateAuthorizationHeader(string token, HttpRequestMessage? request = null)
        {
            if (request is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


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

        public string? GetUserName()
        {
            return _userPermission?.UserName;
        }

        public bool IsAuthorized()
        {
            return _userPermission != null;
        }

        private async Task SaveSessionAsync(UserSession session)
        {
            _userPermission = session ?? throw new Exception("Invalid session!");

            await RemoveCookieAsync();
            await SaveCookieAsync(_userPermission);

            UpdateAuthorizationHeader(_userPermission.AccessToken);
        }

        private async Task RemoveCookieAsync()
        {
            await _localStorageService.RemoveItem("Session");
        }

        private async Task SaveCookieAsync(UserSession permission)
        {
            await _localStorageService.SetItem("Session", permission);
        }

        private async Task<UserSession?> ReadSessionFromStorageAsync()
        {
            try
            {
                var session = await _localStorageService.GetItem<UserSession>("Session");
                return session ?? default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
