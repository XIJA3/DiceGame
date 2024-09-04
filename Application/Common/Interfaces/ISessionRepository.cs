using Domain.Models;

namespace ApplicationTemplate.Infrastructure.Repository
{
    public interface ISessionRepository
    {
        Task AddSessionAsync(Session session);
        Task UpdateSessionAsync(Session session);
        Task AddSessionPlayAsync(SessionPlay sessionPlay);
        Task UpdateSessionPlayAsync(SessionPlay sessionPlay);
        Task AddUserInfoAsync(UserInfo userInfo);
        Task<Session?> GetSessionByIdAsync(long sessionId);
        Task<SessionPlay?> GetSessionPlayByIdAsync(long sessionPlayId);
    }
}
