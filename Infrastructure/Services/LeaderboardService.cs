using ApplicationTemplate.Server.Common.Interfaces;
using DataTransferModels.DTO;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTemplate.Infrastructure.Services
{
    public class LeaderboardService(IApplicationDbContext context) : ILeaderboardService
    {
        private readonly IApplicationDbContext _context = context;

        private const int _leaderboardResultCount = 100;

        public async Task<List<UserLeaderboardDto>> GetTopPlayersByScoreAsync()
        {
            return await _context.UserInfos
                .GroupBy(ui => new { ui.UserId, ui.User.UserName })
                .Select(g => new UserLeaderboardDto
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    TotalScore = g.Sum(ui => ui.Score)
                })
                .OrderByDescending(ul => ul.TotalScore)
                .Take(_leaderboardResultCount)
                .ToListAsync();
        }
    }
}
