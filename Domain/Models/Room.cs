using Domain.Common;
using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Room : BaseEntity, IIdentifiable
    {
        public long User1Id { get; set; }
        public User User1 { get; set; } = null!;
        public long User2Id { get; set; }
        public User User2 { get; set; } = null!;
        public Session? Session { get; set; } = null!;
    }
}
