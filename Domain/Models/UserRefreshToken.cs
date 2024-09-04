using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserRefreshToken
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
