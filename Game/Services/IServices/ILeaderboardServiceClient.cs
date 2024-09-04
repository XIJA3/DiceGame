using DataTransferModels.DTO;

namespace Game.Services.IServices
{
    public interface ILeaderboardServiceClient
    {
        Task<List<UserLeaderboardDto>?> GetTopPlayersByScoreAsync();
    }
}
