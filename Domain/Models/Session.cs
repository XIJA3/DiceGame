using Domain.Common;
using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Session : BaseEntity, IIdentifiable
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long RoomId { get; set; }
        public Room Room { get; set; }= null!;
        public ICollection<SessionPlay> Plays { get; set; } = [];
    }
}
