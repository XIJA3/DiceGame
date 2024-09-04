using ApplicationTemplate.Server.Common.Interfaces;

namespace ApplicationTemplate.Infrastructure.Repository
{

    public class RefreshTokenRepository(IApplicationDbContext context) : IRefreshTokenRepository
    {
        private readonly IApplicationDbContext _context = context;

        public UserRefreshToken AddUserRefreshTokens(UserRefreshToken refreshToken)
        {
            _context.UserRefreshTokens.Add(refreshToken);
            _context.SaveChanges();
            return refreshToken;
        }

        public void DeleteUserRefreshTokens(string username, string refreshToken)
        {
            var item = _context.UserRefreshTokens.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _context.UserRefreshTokens.Remove(item);
            }
        }

        public bool IsValidRefreshToken(string username, string refreshToken)
        {
            var userRefreshToken = _context.UserRefreshTokens
                .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() &&
                                     x.RefreshToken == refreshToken &&
                                     x.IsActive == true);

            return !string.IsNullOrWhiteSpace(userRefreshToken?.RefreshToken);
        }
    }
}
