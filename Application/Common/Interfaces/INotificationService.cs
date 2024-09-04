
using DataTransferModels.Clients;

namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface INotificationService
    {
        void SendToAll(Action<IApplicationClient> action);
        void SendToUser(Action<IApplicationClient> action, string connectionId);
    }
}
