using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Services.IServices
{
    public interface IHubService
    {
        Task<HubConnection> GetHubConnectionAsync();
        Task ConnectToHubAsync();
        Task DisconnectFromHubAsync();
    }
}
