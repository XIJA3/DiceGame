using Microsoft.EntityFrameworkCore;

namespace ApplicationTemplate.Infrastructure.Repository
{
    public class SessionRepository(IApplicationDbContext context) : ISessionRepository
    {
        private readonly IApplicationDbContext _context = context;

        public async Task AddSessionAsync(Session session)
        {
            _context.Sessions.Add(session);
            _context.SaveChanges();
        }

        public async Task UpdateSessionAsync(Session session)
        {
            _context.Sessions.Update(session);
            _context.SaveChanges();
        }

        public async Task AddSessionPlayAsync(SessionPlay sessionPlay)
        {
            _context.SessionPlays.Add(sessionPlay);
            _context.SaveChanges();
        }

        public async Task UpdateSessionPlayAsync(SessionPlay sessionPlay)
        {
            _context.SessionPlays.Update(sessionPlay);
            _context.SaveChanges();
        }

        public async Task AddUserInfoAsync(UserInfo userInfo)
        {
            _context.UserInfos.Add(userInfo);
            _context.SaveChanges();
        }

        public async Task<Session?> GetSessionByIdAsync(long sessionId)
        {
            return await _context.Sessions
                .Include(s => s.Plays)
                .ThenInclude(p => p.UserInfos)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<SessionPlay?> GetSessionPlayByIdAsync(long sessionPlayId)
        {
            return await _context.SessionPlays
                .Include(sp => sp.UserInfos)
                .FirstOrDefaultAsync(sp => sp.Id == sessionPlayId);
        }
    }
}
