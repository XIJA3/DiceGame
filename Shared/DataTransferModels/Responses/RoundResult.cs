using DataTransferModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Models
{
    public class RoundResult
    {
        public long YourTotalScore { get; set; }
        public long YourRemainingPlayCount { get; set; }
        public long OpponentsTotalScore { get; set; }
        public long OpponentsRemainingPlayCount { get; set; }
        public int DiceResult { get; set; } 
        public bool IsYourTurn { get; set; }
    }
}

