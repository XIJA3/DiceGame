using MediatR;

namespace ApplicationTemplate.Server.Commands
{
    public record NotReadyCommand(IUser User) : IRequest { }
}

