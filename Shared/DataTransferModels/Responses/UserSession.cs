using DataTransferModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferModels.Responses
{
    public class UserSession(string accessToken, string userName)
    {
        public string AccessToken { get; set; } = accessToken;
        public string UserName { get; set; } = userName;
    };
}
