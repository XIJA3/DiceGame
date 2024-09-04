using System.Reflection;
using Domain.IRepository;
using Domain.Models.DbEnums;
using Microsoft.EntityFrameworkCore;
using NextLevel.DBContext;

namespace ApplicationTemplate.Infrastructure.Data;

public class ApplicationDbContext : NLDBContext<ApplicationDbContext>, IApplicationDbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionPlay> SessionPlays => Set<SessionPlay>();
    public DbSet<UserInfo> UserInfos => Set<UserInfo>();
    public DbSet<PlayResult> PlayResults => Set<PlayResult>();
    
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<AuthorizationLog> AuthorizationLogs => Set<AuthorizationLog>();
    public DbSet<AccountType> AccountTypes => Set<AccountType>();

}
