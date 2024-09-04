using Domain.Common;
using Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class SessionPlay : BaseEntity, IIdentifiable
    {
        public bool IsFinished { get; set; }
        public long SessionId { get; set; }
        public Session Session { get; set; } = null!;
        public ICollection<UserInfo> UserInfos { get; set; } = [];
    }
}
