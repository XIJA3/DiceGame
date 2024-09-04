using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Services.IServices
{
    public interface IBaseHubService
    {
        Task ConnectToHubAsync();
        Task DisconnectFromHubAsync();
        HubConnectionState ConnectionState { get;  }
        HubConnection HubConnection { get; }
    }
}
