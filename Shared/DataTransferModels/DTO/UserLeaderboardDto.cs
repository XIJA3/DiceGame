namespace DataTransferModels.DTO
{
    public class UserLeaderboardDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long TotalScore { get; set; }
    }
}
