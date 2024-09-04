using MediatR;

namespace ApplicationTemplate.Server.Commands
{
    public record ReadyCommand(IUser User) : IRequest { }
}

