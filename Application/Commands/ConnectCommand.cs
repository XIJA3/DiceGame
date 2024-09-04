using DataTransferModels.Clients;
using MediatR;

namespace ApplicationTemplate.Server.Commands
{
    public record ConnectCommand(IUser User) : IRequest { }
}

