using DataTransferModels.Requests;

namespace DataTransferModels.Clients
{
    public interface IApplicationHub
    {
        Task JoinRoomAsync();
        Task RematchAsync(long sessionId);
        Task CancelRematchAsync(long sessionId);
        Task LeaveRoomAsync(long sessionId);
        Task ReadyAsync();
        Task NotReadyAsync();
        Task ProccessTurnAsync(ProccessTurnRequest request);
    }
}
