using Enums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferModels.Requests
{
    public record ProccessTurnRequest(long SessionId, long SessionPlayId, DiceSetTypes DiceSetType) { }
}
