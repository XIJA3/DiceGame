using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Models
{
    public class GamePlayer(IUser user)
    {
        public IUser User = user;
        public PlayerStatuses Status { get; set; } = PlayerStatuses.Offline;
        public PlayerRoomStatus RoomStatus { get; set; } = PlayerRoomStatus.NotReady;
    }
}
