using DataTransferModels.Clients;
using DataTransferModels.DTO;
using ApplicationTemplate.Server.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Net.Sockets;
using Web.Hubs;

namespace Web.Services
{
    public class NotificationService(IHubContext<ApplicationHub, IApplicationClient> hubContext) : INotificationService
    {
        protected readonly IHubContext<ApplicationHub, IApplicationClient> _hubContext = hubContext;
        private IApplicationClient AllClients => _hubContext.Clients.All;

        public void SendToAll(Action<IApplicationClient> action)
        {
            _ = Task.Run(() =>
            {
                action(AllClients);
            });
        }

        public void SendToUser(Action<IApplicationClient> action, string connectionId)
        {
            _ = Task.Run(() =>
            {
                action(_hubContext.Clients.Client(connectionId));
            });
        }
    }
}
