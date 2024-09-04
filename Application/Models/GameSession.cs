using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Models
{
    public class GameSession(long id)
    {
        public long Id { get; set; } = id;
        public DateTime StartTime { get; set; }
        public GamePlayer User1 { get; set; } = null!;
        public GamePlayer User2 { get; set; } = null!;
        public List<GameSessionPlay > Plays { get; set; } = [];
    }
}
