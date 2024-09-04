using DataTransferModels.DTO;
using Game.Services.IServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Game.Services
{
    /// <summary>
    /// Provides methods to interact with the leaderboard service API.
    /// </summary>
    public class LeaderboardServiceClient(IConfiguration configuration) : ILeaderboardServiceClient
    {
        // Configuration for retrieving application settings
        private readonly IConfiguration _configuration = configuration;

        // HttpClient instance used for making HTTP requests
        private HttpClient _httpClient;

        /// <summary>
        /// Gets the HttpClient instance configured with the base URL from the configuration.
        /// </summary>
        internal HttpClient HttpClient
        {
            get
            {
                // Retrieve the base URL from configuration
                var baseUrl = _configuration?["ServerBaseUri"]
                    ?? throw new InvalidOperationException("ServerBaseUri is not configured.");

                // Initialize and configure HttpClient if not already done
                return _httpClient ??= new HttpClient()
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromMinutes(5)
                };
            }
        }

        /// <summary>
        /// Asynchronously retrieves the top players by score from the leaderboard API.
        /// </summary>
        /// <returns>A list of UserLeaderboardDto representing top players if the request is successful; otherwise, null.</returns>
        public async Task<List<UserLeaderboardDto>?> GetTopPlayersByScoreAsync()
        {
            var url = "/api/leaderboard/GetTopPlayersByScore";

            try
            {
                // Send GET request to the leaderboard API
                var response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Read and return the list of top players from the response
                return await response.Content.ReadFromJsonAsync<List<UserLeaderboardDto>>();
            }
            catch (HttpRequestException)
            {
                // Handle HTTP request exceptions (e.g., network errors)
                return null;
            }
            catch (Exception)
            {
                // Handle other exceptions (e.g., serialization issues)
                return null;
            }
        }
    }
}
