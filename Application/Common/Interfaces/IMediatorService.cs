using ApplicationTemplate.Server.Commands;

namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface IMediatorService
    {
        Task ConnectAsync(ConnectCommand request);
        Task DisconnectAsync(DisconnectCommand request);
        Task JoinRoomAsync(JoinRoomCommand request);
        Task RematchAsync(RematchCommand request);
        Task CancelRematchAsync(CancelRematchCommand request);
        Task LeaveRoomAsync(LeaveRoomCommand request);
        Task ReadyAsync(ReadyCommand request);
        Task NotReadyAsync(NotReadyCommand request);
        Task ProccessTurnAsync(ProccessTurnCommand request);
    }
}
