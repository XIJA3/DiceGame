using Domain.Common;
using Domain.Contracts;
using Domain.Models.DbEnums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class UserInfo : BaseEntity, IIdentifiable
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long Score { get; set; }
        public long PlayResultId { get; set; }
        public long SessionPlayId { get; set; }
        public SessionPlay SessionPlay { get; set; } = null!;
    }
}
