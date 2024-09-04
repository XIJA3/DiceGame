using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface IRefreshTokenRepository
    {
            UserRefreshToken AddUserRefreshTokens(UserRefreshToken user);

            bool IsValidRefreshToken(string username, string refreshtoken);

            void DeleteUserRefreshTokens(string username, string refreshToken);
    }
}
