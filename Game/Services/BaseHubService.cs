using Client.Middlewares;
using Client.Services.IServices;
using DataTransferModels.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Client.Services
{
    /// <summary>
    /// Manages SignalR Hub connection and provides methods for connecting and disconnecting.
    /// </summary>
    public class BaseHubService(IAuthService authManager, IConfiguration configuration, NavigationManager navigationManager) : IBaseHubService
    {
        // Gets the base URL for the SignalR hub from configuration
        private string BaseUrl => _configuration?["HubBaseURI"]
            ?? throw new InvalidOperationException("HubBaseURI is not configured.");

        // SemaphoreSlim for ensuring thread-safe operations on the connection
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        // Authentication manager for handling tokens
        private readonly IAuthService _authManager = authManager;
        // Navigation manager for handling navigation events
        private readonly NavigationManager _navigationManager = navigationManager;
        // Configuration for retrieving settings
        private readonly IConfiguration _configuration = configuration;

        // Middleware handler for managing HTTP requests
        private ClientMiddleware HttpClientHandler => new(new HttpClientHandler(), _authManager.ReadSessionAsync, _navigationManager);

        /// <summary>
        /// Gets the current state of the Hub connection.
        /// </summary>
        public HubConnectionState ConnectionState => HubConnection?.State ?? HubConnectionState.Disconnected;

        // The SignalR Hub connection
        private HubConnection? _hubConnection;
        /// <summary>
        /// Gets the SignalR Hub connection. Creates a new connection if it does not exist.
        /// </summary>
        public HubConnection HubConnection
        {
            get
            {
                try
                {
                    return _hubConnection ??= CreateNewHubConnection();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to initialize HubConnection.", ex);
                }
            }
        }

        /// <summary>
        /// Creates a new SignalR Hub connection with specified configuration.
        /// </summary>
        private HubConnection CreateNewHubConnection()
        {
            return new HubConnectionBuilder()
                .WithUrl(BaseUrl, options =>
                {
                    options.AccessTokenProvider = async () => await _authManager.GetAccessTokenAsync();
                    options.HttpMessageHandlerFactory = _ => HttpClientHandler;
                })
                .WithAutomaticReconnect()
                .Build();
        }

        /// <summary>
        /// Connects to the SignalR Hub if not already connected.
        /// </summary>
        public async Task ConnectToHubAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (ConnectionState != HubConnectionState.Connected)
                {
                    if (_hubConnection == null || _hubConnection.State == HubConnectionState.Disconnected)
                    {
                        _hubConnection = null; // Ensure we're not reusing a disposed connection
                        _hubConnection = CreateNewHubConnection(); // Recreate the connection
                    }
                    await HubConnection.StartAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to start the Hub connection.", ex);
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        /// <summary>
        /// Disconnects from the SignalR Hub and disposes of the connection.
        /// </summary>
        public async Task DisconnectFromHubAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (HubConnection != null)
                {
                    await HubConnection.DisposeAsync();
                    _hubConnection = null; // Ensure the disposed connection is not reused
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
    }
}
