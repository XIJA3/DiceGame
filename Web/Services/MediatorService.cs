using DataTransferModels.Responses;
using ApplicationTemplate.Server.Common.Interfaces;
using MediatR;
using ApplicationTemplate.Server.Commands;

namespace Web.Services
{
    public class MediatorService(ISender sender) : IMediatorService
    {
        private readonly ISender _sender = sender;

        public async Task ConnectAsync(ConnectCommand request)
        {
            await _sender.Send(request);
        }

        public async Task DisconnectAsync(DisconnectCommand request)
        {
            await _sender.Send(request);
        }

        public async Task JoinRoomAsync(JoinRoomCommand request)
        {
            await _sender.Send(request);
        }

        public async Task LeaveRoomAsync(LeaveRoomCommand request)
        {
            await _sender.Send(request);
        }

        public async Task ReadyAsync(ReadyCommand request)
        {
            await _sender.Send(request);
        }

        public async Task NotReadyAsync(NotReadyCommand request)
        {
            await _sender.Send(request);
        }

        public async Task ProccessTurnAsync(ProccessTurnCommand request)
        {
            await _sender.Send(request);
        }

        public async Task RematchAsync(RematchCommand request)
        {
            await _sender.Send(request);
        }

        public async Task CancelRematchAsync(CancelRematchCommand request)
        {
            await _sender.Send(request);
        }
    }
}
