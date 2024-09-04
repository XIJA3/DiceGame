using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class AuthorizationLog : BaseEntity
    {
        public long UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceType { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }

        public User User { get; set; } = null!;
    }
}
