using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferModels.Responses
{
    public class GameStartResponse
    {
        public long SessionId { get; set; }
        public long SessionPlayId { get; set; }
        public string OpponentsName { get; set; } = string.Empty;
        public long MaximumPlayCount { get; set; }
        public bool IsYourTurn { get; set; }
    }
}
