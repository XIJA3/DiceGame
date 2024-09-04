using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Commands
{
    public record DisconnectCommand(IUser User, Exception? exception) : IRequest { }
}
