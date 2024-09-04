using ApplicationTemplate.Server.Models;
using ApplicationTemplate.Server.Services.IServices;
using Domain.IRepository;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Services
{
    public class PlayerManager : IPlayerManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationService _notificationService;
        private readonly ConcurrentDictionary<long, GamePlayer> _players = new();
        private readonly Timer _timer;

        private const int _reconnectWaitingSeconds = 30;

        // Property to get the count of players
        public int PlayerCount => _players.Count;

        public PlayerManager(IServiceProvider serviceProvider, INotificationService notificationService)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;

            // Initialize the timer to tick every 5 seconds.
            _timer = new Timer(async (obj) => await Tick(obj), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        // Method to update online player count for all players
        private async Task Tick(object? state)
        {
            foreach (var player in _players.Values)
                await player.User.Client.OnlinePlayerCount(PlayerCount);
        }

        // Event triggered when a player is added
        public event Action<GamePlayer>? OnAdd;
        // Event triggered when a player is removed
        public event Action<GamePlayer>? OnRemove;
        // Event triggered when a player reconnects
        public event Action<GamePlayer>? OnReconnect;
        // Event triggered when a player disconnects
        public event Action<GamePlayer>? OnDisconnect;

        // Method to get a player by ID
        public async Task<GamePlayer> GetPlayer(long userId)
        {
            return await TryGetPlayer(userId) ?? throw new Exception("User Not Found!");
        }

        // Method to try to get a player by ID (returns null if not found)
        public Task<GamePlayer?> TryGetPlayer(long userId)
        {
            _players.TryGetValue(userId, out var onlineUser);
            return Task.FromResult(onlineUser);
        }

        // Method to add a player
        public async Task AddPlayer(IUser user)
        {
            using var scope = _serviceProvider.CreateScope();

            var gamePlayer = await TryGetPlayer(user.Id);

            if (gamePlayer is not null)
            {
                if (gamePlayer.Status == PlayerStatuses.Online)
                {
                    await gamePlayer.User.Client.GameIsOpenInOtherWindow();
                }
            }
            else
            {
                var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();

                var dbPlayer = await playerRepository.GetPlayerById(user.Id)
                    ?? throw new Exception("User does not exists");

                gamePlayer = new GamePlayer(user);

                _players[user.Id] = gamePlayer;

                foreach (var player in _players.Values)
                    await player.User.Client.OnlinePlayerCount(PlayerCount);
            }

            gamePlayer.Status = PlayerStatuses.Online;

            OnAdd?.Invoke(gamePlayer);
        }

        // Method to remove a player
        public async Task RemovePlayer(IUser user)
        {
            var gamePlayer = _players[user.Id];

            OnRemove?.Invoke(gamePlayer);

            _players.TryRemove(user.Id, out _);

            foreach (var player in _players.Values)
                await player.User.Client.OnlinePlayerCount(PlayerCount);
        }

        // Method to get all players
        public Task<IReadOnlyCollection<GamePlayer>> GetAllPlayers()
        {
            return Task.FromResult<IReadOnlyCollection<GamePlayer>>(_players.Values.ToList());
        }
    }
}
