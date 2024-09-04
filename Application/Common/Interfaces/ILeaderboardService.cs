using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<UserLeaderboardDto>> GetTopPlayersByScoreAsync();
    }
}
