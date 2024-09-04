using MediatR;

namespace ApplicationTemplate.Server.Commands
{
    public record LeaveRoomCommand(IUser User,long SessionId) : IRequest { }
}

