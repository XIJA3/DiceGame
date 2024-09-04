using Domain.Models;
using Domain.Models.DbEnums;
using Microsoft.EntityFrameworkCore;

namespace Domain.IRepository;

public interface IApplicationDbContext : IDisposable
{
    DbSet<UserInfo> UserInfos{ get; }
    DbSet<SessionPlay> SessionPlays{ get; }
    DbSet<Room> Rooms { get; }
    DbSet<Session> Sessions{ get; }
    DbSet<User> Users { get; }
    DbSet<PlayResult> PlayResults { get; }

    DbSet<UserRefreshToken> UserRefreshTokens { get; }
    DbSet<AuthorizationLog> AuthorizationLogs { get; }
    DbSet<AccountType> AccountTypes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    int SaveChanges();
}
