using Enums.Enums;

namespace ApplicationTemplate.Server.Models
{
    public class GameFinishResult
    {
        public long YourTotalScore { get; set; }
        public long OpponentsTotalScore { get; set; }
        public GameResults YourGameResult { get; set; }
    }
}

