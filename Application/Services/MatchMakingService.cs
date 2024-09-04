using ApplicationTemplate.Server.Common.Interfaces;
using ApplicationTemplate.Server.Helpers;
using ApplicationTemplate.Server.Models;
using ApplicationTemplate.Server.Services.IServices;
using Domain.IRepository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Numerics;

namespace ApplicationTemplate.Server.Services
{
    /// <summary>
    /// Service for matchmaking, managing game rooms and player status in the rooms.
    /// </summary>
    public class MatchMakingService : IMatchMakingService
    {
        private readonly IScopedServiceExecutor _scopedServiceExecutor;
        private readonly IPlayerManager _playerManager;
        private readonly ISessionService _sessionService;
        private readonly List<GameRoom> _rooms = new();  // List of game rooms
        private readonly object _lock = new(); // Lock object for thread safety

        public MatchMakingService(
            ISessionService sessionService,
            IPlayerManager playerManager,
            IScopedServiceExecutor scopedServiceExecutor)
        {
            _scopedServiceExecutor = scopedServiceExecutor;
            _playerManager = playerManager;
            _sessionService = sessionService;

            // Event handler for player removal
            _playerManager.OnRemove += (gameplayer) =>
            {
                var room = _rooms.FirstOrDefault(r => r.Players.Any(p => p.User.Id == gameplayer.User.Id));

                if (room?.Session?.Id is not null)
                    LeaveRoom(gameplayer.User, room.Session.Id);
                else
                    RemovePlayer(gameplayer.User);
            };
        }

        /// <summary>
        /// Allows a user to join a game room, either creating a new room or joining an existing one.
        /// </summary>
        public async Task JoinRoom(IUser user)
        {
            var player = await _playerManager.GetPlayer(user.Id);

            lock (_lock)
            {
                // Check if the user is already in any room
                if (_rooms.Any(room => room.Players.Any(gameplayer => gameplayer.User.Id == user.Id)))
                {
                    user.Client.AlreadyInRoom();
                    return;
                }

                // Find an existing room with space or create a new one
                var freeRooms = _rooms.Where(r => !r.IsSessionStarted && r.Players.Count > 0).ToList();
                GameRoom gameRoom;

                if (freeRooms.Count != 0)
                {
                    gameRoom = RandomExtensions.GetRandomItem(freeRooms);
                }
                else
                {
                    gameRoom = new GameRoom();
                    _rooms.Add(gameRoom);
                }

                gameRoom.Players.Add(player);

                if (gameRoom.Players.Count == 2)
                {
                    foreach (var gamePlayer in gameRoom.Players)
                    {
                        gamePlayer.User.Client.StartedMatchMaking();
                    }
                }
                else
                {
                    user.Client.WaitingOpponent();
                }
            }

            return;
        }

        /// <summary>
        /// Handles a user leaving a room and ends the session if applicable.
        /// </summary>
        public Task LeaveRoom(IUser user, long sessionId)
        {
            lock (_lock)
            {
                _sessionService.EndSession(sessionId, user);
                RemovePlayer(user);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a player from a room and handles room and session updates.
        /// </summary>
        public Task RemovePlayer(IUser user)
        {
            lock (_lock)
            {
                var room = _rooms.FirstOrDefault(r => r.Players.Any(p => p.User.Id == user.Id));

                if (room == null)
                {
                    return Task.CompletedTask;
                }

                var playerToRemove = room.Players.FirstOrDefault(p => p.User.Id == user.Id);

                if (playerToRemove != null)
                {
                    playerToRemove.User.Client.YouLeftRoom();
                    playerToRemove.RoomStatus = PlayerRoomStatus.NotReady;
                    room.Players.Remove(playerToRemove);
                    _sessionService.RemovePlayer(user);
                }

                var secondPlayer = room.Players.FirstOrDefault(x => x.User.Id != user.Id);

                if (secondPlayer != null)
                {
                    secondPlayer.User.Client.OpponentDisconnected();
                    secondPlayer.RoomStatus = PlayerRoomStatus.NotReady;
                    secondPlayer.User.Client.YouLeftRoom();
                    room.Players.Remove(secondPlayer);
                    _sessionService.RemovePlayer(secondPlayer.User);
                }

                // Remove the room if empty
                if (room.Players.Count == 0)
                {
                    _rooms.Remove(room);
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets a player's status to ready and starts a session if all players are ready.
        /// </summary>
        public Task Ready(IUser user) => TryStartSession(user, PlayerRoomStatus.Ready);

        /// <summary>
        /// Sets a player's status to not ready.
        /// </summary>
        public Task NotReady(IUser user) => TryStartSession(user, PlayerRoomStatus.NotReady);

        private Task TryStartSession(IUser user, PlayerRoomStatus status)
        {
            lock (_lock)
            {
                SetReadyStatus(user, status);

                var gameRoom = _rooms.FirstOrDefault(r => r.Players.Any(gameplayer => gameplayer.User.Id == user.Id));

                if (gameRoom == null)
                {
                    user.Client.YouLeftRoom();
                    return Task.CompletedTask;
                }

                if (gameRoom.Players.All(x => x.RoomStatus == PlayerRoomStatus.Ready))
                {
                    var secondPlayer = gameRoom.Players.First(gameplayer => gameplayer.User.Id != user.Id);

                    gameRoom.Id = AddRoomToDb(user.Id, secondPlayer.User.Id).Result;
                    gameRoom.Session = _sessionService.StartSession(gameRoom).Result;
                    gameRoom.IsSessionStarted = true;
                }
            }

            return Task.CompletedTask;
        }

        private async Task SetReadyStatus(IUser user, PlayerRoomStatus status)
        {
            lock (_lock)
            {
                var gameRoom = _rooms.FirstOrDefault(r => r.Players.Any(gameplayer => gameplayer.User.Id == user.Id));
                if (gameRoom == null)
                {
                    user.Client.YouLeftRoom();
                    return;
                }

                var player = _playerManager.GetPlayer(user.Id).Result;
                if (player == null)
                {
                    user.Client.PlayerNotFound();
                    return;
                }

                player.RoomStatus = status;
                player.User.Client.YourRoomStatus(status);

                var secondPlayer = gameRoom.Players.FirstOrDefault(gameplayer => gameplayer.User.Id != user.Id);

                secondPlayer?.User.Client.OpponentsRoomStatus(player.RoomStatus);
            }
        }

        /// <summary>
        /// Starts a rematch by setting the player status to ready and starts the session if all players are ready.
        /// </summary>
        public Task Rematch(IUser user, long sessionId) => SetRematchStatus(user, sessionId, PlayerRoomStatus.Ready);

        /// <summary>
        /// Cancels a rematch by setting the player status to not ready.
        /// </summary>
        public Task CancelRematch(IUser user, long sessionId) => SetRematchStatus(user, sessionId, PlayerRoomStatus.NotReady);

        /// <summary>
        /// Sets the rematch status for a player and starts the game if all players are ready.
        /// </summary>
        public Task SetRematchStatus(IUser user, long sessionId, PlayerRoomStatus status)
        {
            lock (_lock)
            {
                SetReadyStatus(user, status);

                var gameRoom = _rooms.FirstOrDefault(r => r.Players.Any(gameplayer => gameplayer.User.Id == user.Id));

                if (gameRoom == null)
                {
                    user.Client.YouLeftRoom();
                    return Task.CompletedTask;
                }

                if (gameRoom.Players.All(x => x.RoomStatus == PlayerRoomStatus.Ready))
                {
                    _sessionService.StartGame(sessionId);
                }
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Adds a room to the database and returns its ID.
        /// </summary>
        private async Task<long> AddRoomToDb(long user1Id, long user2Id)
        {
            var dbRoom = new Room
            {
                User1Id = user1Id,
                User2Id = user2Id,
            };

            _scopedServiceExecutor.Execute<IApplicationDbContext>(context =>
            {
                context.Rooms.Add(dbRoom);
                context.SaveChanges();
            });

            return dbRoom.Id;
        }
    }
}
