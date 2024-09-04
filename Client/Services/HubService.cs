using Client.Middlewares;
using Client.Services.IServices;
using DataTransferModels.Responses;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Services;

public class HubService(IAuthService authManager, IConfiguration configuration) : IHubService
{
    private string BaseUrl => _configuration?["HubBaseURI"]
        ?? throw new InvalidOperationException("HubBaseURI is not configured.");

    private HubConnection? _hubConnection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly IAuthService _authManager = authManager;
    private readonly IConfiguration _configuration = configuration;

    private ClientMiddleware _httpClientHandler;
    private ClientMiddleware HttpClientHandler
    {
        get
        {
            return _httpClientHandler ??= new ClientMiddleware(new HttpClientHandler(), _authManager.ReadSessionAsync);
        }
    }


    public async Task<HubConnection> GetHubConnectionAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            _hubConnection ??= new HubConnectionBuilder()
                    .WithUrl(BaseUrl, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(_authManager.AccessToken);
                        options.HttpMessageHandlerFactory = _ => HttpClientHandler;
                    })
                    .WithAutomaticReconnect()
                    .Build();

            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await _hubConnection.StartAsync();
            }

            return _hubConnection;
        }
        catch (Exception ex)
        {
            // Consider logging or wrapping the exception with more context
            throw new InvalidOperationException("Unable to start the Hub connection.", ex);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task ConnectToHubAsync()
    {
        await GetHubConnectionAsync();
    }

    public async Task DisconnectFromHubAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
