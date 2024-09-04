using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ClientError : BaseEntity
    {
        public long UserId { get; set; }
        public string Error { get; set; }
        public string Source { get; set; }
        public User User { get; set; }
    }
}
