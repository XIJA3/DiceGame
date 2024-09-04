using ApplicationTemplate.Server.Helpers;
using Domain.Common;
using Domain.Contracts;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Models
{
    public class GameRoom()
    {
        public long Id { get; set; }
        public List<GamePlayer> Players { get; set; } = new List<GamePlayer>(2);
        public GameSession? Session { get; set; }
        public bool IsSessionStarted { get; set; }
    }
}
