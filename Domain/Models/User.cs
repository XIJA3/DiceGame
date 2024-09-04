using Domain.Common;
using Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User : BaseEntity, IIdentifiable
    {
        public string UserName { get; set; } = null!;
        public DateTime RegisteredOn { get; set; }


        public ICollection<AuthorizationLog> AuthorizationLogs { get; set; } = null!;
        public ICollection<ClientError> ClientErrors { get; set; } = null!;
    }
}
