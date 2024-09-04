using Domain.IRepository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTemplate.Infrastructure.Repository
{
    public class PlayerRepository(IApplicationDbContext context) : IPlayerRepository
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<User> CreateAsync(string name)
        {
            var player = new User
            {
                UserName = name,
                RegisteredOn = DateTime.Now,
            };

            await _context.Users.AddAsync(player);

            _context.SaveChanges();

            return player;
        }

        public async Task<bool> DeleteAsync(long userId)
        {
            var player = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (player is null)
                return false;

            _context.Users.Remove(player);
            _context.SaveChanges();
            return true;
        }

        public async Task<User?> GetPlayerById(long userId)
        {
            var player = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return player;
        }

        public async Task<User?> GetPlayerByName(string userName)
        {
            var player = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            return player;
        }
    }
}
