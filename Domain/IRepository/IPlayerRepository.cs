using Domain.Models;

namespace Domain.IRepository
{
    public interface IPlayerRepository
    {
        Task<User?> GetPlayerById(long userId);
        Task<User?> GetPlayerByName(string userName);
        Task<User> CreateAsync(string name);
        Task<bool> DeleteAsync(long userId);
    }
}
