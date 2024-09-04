using MediatR;

namespace ApplicationTemplate.Server.Commands
{
    public record JoinRoomCommand(IUser User) : IRequest { }
}

