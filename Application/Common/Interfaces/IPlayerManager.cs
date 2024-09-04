namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface IPlayerManager
    {
        public event Action<GamePlayer>? OnAdd;
        public event Action<GamePlayer>? OnRemove;
        public event Action<GamePlayer>? OnReconnect;
        public event Action<GamePlayer>? OnDisconnect;

        int PlayerCount { get; }

        Task<GamePlayer> GetPlayer(long userId);
        Task<GamePlayer?> TryGetPlayer(long userId);
        Task AddPlayer(IUser user);
        Task RemovePlayer(IUser user);
        Task<IReadOnlyCollection<GamePlayer>> GetAllPlayers();
    }
}
