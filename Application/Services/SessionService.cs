using ApplicationTemplate.Infrastructure.Repository;
using ApplicationTemplate.Server.Common.Interfaces;
using ApplicationTemplate.Server.Helpers;
using ApplicationTemplate.Server.Models;
using ApplicationTemplate.Server.Services.IServices;
using DataTransferModels.Responses;
using DataTransferModels.ValueObjects;
using Domain.Models;
using Domain.Models.DbEnums;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using static ApplicationTemplate.Server.Models.GameSessionPlay;

namespace ApplicationTemplate.Server.Services
{
    public class SessionService : ISessionService
    {
        private readonly IDiceService _diceService; // Service for rolling dice
        private readonly IScopedServiceExecutor _scopedServiceExecutor; // Executor for scoped service operations
        private readonly ConcurrentDictionary<long, GameSession> _sessions = new(); // Dictionary to store active sessions
        private const long _maximumPlayCount = 3; // Maximum number of plays per player
        private const long _maximumWinPossibe = 24; // Maximum possible score

        public SessionService(
            IDiceService diceService,
            IScopedServiceExecutor scopedServiceExecutor)
        {
            _diceService = diceService;
            _scopedServiceExecutor = scopedServiceExecutor;
        }

        // Attempt to retrieve a session by its ID
        private GameSession? TryGetSession(long sessionId) =>
            _sessions.TryGetValue(sessionId, out var session) ? session : null;

        // Retrieve a session by its ID, throws if not found
        private GameSession GetSession(long sessionId) =>
            TryGetSession(sessionId) ?? throw new InvalidOperationException("Session not found");

        // Start a new session for a given game room
        public async Task<GameSession> StartSession(GameRoom gameRoom)
        {
            // Create a new session
            var session = new Session
            {
                StartTime = DateTime.UtcNow,
                RoomId = gameRoom.Id,
            };

            // Add the session to the repository
            await _scopedServiceExecutor.ExecuteAsync<ISessionRepository>(async repo =>
            {
                await repo.AddSessionAsync(session);
            });

            // Create a new game session and add it to the dictionary
            var gameSession = new GameSession(session.Id)
            {
                StartTime = DateTime.UtcNow,
                User1 = gameRoom.Players.First(),
                User2 = gameRoom.Players.Last()
            };

            _sessions[session.Id] = gameSession;

            // Start the game
            await StartGame(session.Id);

            return gameSession;
        }

        // End a session and handle the result based on the loser
        public async Task EndSession(long sessionId, IUser loser)
        {
            _sessions.TryRemove(sessionId, out var session);

            await _scopedServiceExecutor.ExecuteAsync<ISessionRepository>(async repo =>
            {
                var dbSession = await repo.GetSessionByIdAsync(sessionId)
                    ?? throw new Exception("Session not found");

                var sessionPlays = dbSession.Plays.ToList();

                // Handle each play in the session
                foreach (var sessionPlay in sessionPlays)
                {
                    var winner = sessionPlay.UserInfos.SingleOrDefault(x => x.UserId != loser.Id)
                        ?? throw new Exception("Winner not found");

                    await EndGame(sessionPlay.Id, winner.Id);
                }

                // Update the session end time
                dbSession.EndTime = DateTime.UtcNow;
                await repo.UpdateSessionAsync(dbSession);
            });
        }

        // Start a new game within an existing session
        public async Task StartGame(long sessionId)
        {
            var session = GetSession(sessionId);

            if (session.User1.RoomStatus == PlayerRoomStatus.Ready && session.User2.RoomStatus == PlayerRoomStatus.Ready)
            {
                var sessionPlay = new SessionPlay
                {
                    SessionId = sessionId,
                };

                await _scopedServiceExecutor.ExecuteAsync<ISessionRepository>(async repo =>
                {
                    await repo.AddSessionPlayAsync(sessionPlay);

                    var gameSessionPlay = new GameSessionPlay(sessionPlay.Id, session.User1, session.User2);

                    session.Plays.Add(gameSessionPlay);
                    OnGameStarted(gameSessionPlay.CurrentPlayerInfo, gameSessionPlay.OpponentPlayerInfo, sessionId, sessionPlay.Id);
                });
            }
        }

        // Notify players that the game has started
        private void OnGameStarted(PlayerInfo currentInfo, PlayerInfo opponentsInfo, long sessionId, long sessionPlayId)
        {
            var gameStartedResponse = new GameStartResponse
            {
                OpponentsName = opponentsInfo.Player.User.UserName,
                SessionId = sessionId,
                SessionPlayId = sessionPlayId,
                MaximumPlayCount = _maximumPlayCount,
                IsYourTurn = true
            };

            currentInfo.Player.User.Client.GameStarted(gameStartedResponse);

            // Notify the opponent player
            gameStartedResponse.OpponentsName = currentInfo.Player.User.UserName;
            gameStartedResponse.IsYourTurn = false;

            opponentsInfo.Player.User.Client.GameStarted(gameStartedResponse);
        }

        // Submit the results of a finished game
        private async Task SubmitFinishedGame(GameSessionPlay sessionPlay, long? winnerId = null)
        {
            PlayerInfo winner;
            PlayerInfo loser;
            GameResults winnerResult = GameResults.Win;
            GameResults loserResult = GameResults.Lose;

            if (winnerId.HasValue)
            {
                winner = sessionPlay.CurrentPlayerInfo.UserId == winnerId
                    ? sessionPlay.CurrentPlayerInfo
                    : sessionPlay.OpponentPlayerInfo;

                loser = winner == sessionPlay.CurrentPlayerInfo
                    ? sessionPlay.OpponentPlayerInfo
                    : sessionPlay.CurrentPlayerInfo;
            }
            else
            {
                if (sessionPlay.CurrentPlayerInfo.Score > sessionPlay.OpponentPlayerInfo.Score)
                {
                    winner = sessionPlay.CurrentPlayerInfo;
                    loser = sessionPlay.OpponentPlayerInfo;
                }
                else if (sessionPlay.OpponentPlayerInfo.Score > sessionPlay.CurrentPlayerInfo.Score)
                {
                    winner = sessionPlay.OpponentPlayerInfo;
                    loser = sessionPlay.CurrentPlayerInfo;
                }
                else
                {
                    winner = sessionPlay.CurrentPlayerInfo;
                    loser = sessionPlay.OpponentPlayerInfo;
                    winnerResult = loserResult = GameResults.Draw;
                }
            }

            var winnerUserInfo = AddUserInfo(winner, PlayResult.Win, sessionPlay.Id);
            var loserUserInfo = AddUserInfo(loser, PlayResult.Lose, sessionPlay.Id);

            await _scopedServiceExecutor.ExecuteAsync<ISessionRepository>(async repo =>
            {
                await UpdateSessionPlay(sessionPlay.Id, winnerUserInfo, loserUserInfo);

                await SubmitFinishedGame(winner, loser, winnerResult);
                await SubmitFinishedGame(loser, winner, loserResult);

                var session = _sessions.Values.FirstOrDefault(x => x.Plays.Any(x => x.Id == sessionPlay.Id));

                if (session != null)
                {
                    session.User1.RoomStatus = PlayerRoomStatus.NotReady;
                    session.User2.RoomStatus = PlayerRoomStatus.NotReady;
                }

                sessionPlay.IsFinished = true;
            });
        }

        // Submit the final results to the players
        private async Task SubmitFinishedGame(PlayerInfo current, PlayerInfo opponent, GameResults yourGameResult)
        {
            var finishResult = new GameFinishResult
            {
                YourTotalScore = current.Score,
                OpponentsTotalScore = opponent.Score,
                YourGameResult = yourGameResult,
            };

            await current.Player.User.Client.YourGameResult(finishResult);
        }

        // Update the session play record
        private async Task UpdateSessionPlay(long sessionPlayId, UserInfo user1Info, UserInfo user2Info)
        {
            await _scopedServiceExecutor.ExecuteAsync<ISessionRepository>(async repo =>
            {
                var dbSessionPlay = await repo.GetSessionPlayByIdAsync(sessionPlayId)
                    ?? throw new Exception("SessionPlay not found");

                dbSessionPlay.IsFinished = true;
                await repo.UpdateSessionPlayAsync(dbSessionPlay);
            });
        }

        // Add user info for a session play
        private UserInfo AddUserInfo(PlayerInfo playerInfo, PlayResult playResult, long sessionPlayId)
        {
            var userInfo = new UserInfo
            {
                UserId = playerInfo.UserId,
                Score = playerInfo.Score,
                PlayResultId = playResult.Id,
                SessionPlayId = sessionPlayId,
            };

            _scopedServiceExecutor.ExecuteAsync<ISessionRepository>(async repo =>
            {
                await repo.AddUserInfoAsync(userInfo);
            });

            return userInfo;
        }

        // End a game play, determining the winner if provided
        public async Task EndGame(long sessionPlayId, long? winnerId = null)
        {
            var sessionPlay = _sessions.Values
                .SelectMany(session => session.Plays)
                .FirstOrDefault(play => !play.IsFinished && play.Id == sessionPlayId)
                ?? throw new Exception("Play not found");

            await SubmitFinishedGame(sessionPlay, winnerId);
        }

        // Retrieve the current session play by its ID
        public async Task<GameSessionPlay> GetCurrentSessionPlay(long sessionId, long sessionPlayId)
        {
            var session = GetSession(sessionId);
            var sessionPlay = session.Plays.FirstOrDefault(x => x.Id == sessionPlayId)
                ?? throw new Exception("Play not found");

            return sessionPlay;
        }

        // Get a session by its ID
        public Task<GameSession?> GetSessionAsync(long sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return Task.FromResult(session);
        }

        public Task<GameSession?> GetSessionAsync(long sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return Task.FromResult(session);
        }

        public async Task ProcessTurnAsync(long sessionId, long sessionPlayId, IUser user, DiceSetTypes diceSetType)
        {
            // Retrieve the current session play details based on sessionId and sessionPlayId
            var sessionPlay = await GetCurrentSessionPlay(sessionId, sessionPlayId);

            // Check if the current player is the one making the request
            if (sessionPlay.CurrentPlayerInfo.Player.User.Id != user.Id)
            {
                // Notify the user that it's not their turn and exit
                user.Client.NotYourTurn();
                return; // Exit the method since it's not this player's turn
            }

            // Roll the dice and get the result
            var rollResult = await _diceService.RollDiceSet(diceSetType);

            // Handle the case where the dice roll "X"
            if (rollResult == -1)
            {
                // Set the player's score to zero and end the game, declaring the opponent as the winner
                sessionPlay.CurrentPlayerInfo.Score = 0;
                await EndGame(sessionPlayId, sessionPlay.OpponentPlayerInfo.Player.User.Id);
                return; // Exit the method after ending the game
            }

            // Update the player's score and play count with the result of the dice roll
            sessionPlay.CurrentPlayerInfo.Score += rollResult;
            sessionPlay.CurrentPlayerInfo.PlayCount++;

            // Check if both players have reached the maximum number of plays
            if (sessionPlay.CurrentPlayerInfo.PlayCount >= _maximumPlayCount &&
                sessionPlay.OpponentPlayerInfo.PlayCount >= _maximumPlayCount)
            {
                // End the game if the maximum play count is reached for both players
                await EndGame(sessionPlayId);
            }
            else
            {
                // Calculate the remaining plays and the possible maximum score for the opponent
                var playLeft = _maximumPlayCount - sessionPlay.CurrentPlayerInfo.PlayCount;
                var opponentsPossibleWin = (playLeft == 0 ? 1 : playLeft) * _maximumWinPossibe;

                // End the game if the current player cannot catch up with the opponent's score
                if (sessionPlay.CurrentPlayerInfo.Score + opponentsPossibleWin < sessionPlay.OpponentPlayerInfo.Score)
                {
                    await EndGame(sessionPlayId, sessionPlay.OpponentPlayerInfo.Player.User.Id);
                }
                else
                {
                    // Send round result updates to both players
                    RoundResult(sessionPlay, rollResult);
                }
            }
        }

        private void RoundResult(GameSessionPlay sessionPlay, int diceResult)
        {
            var currentInfo = sessionPlay.CurrentPlayerInfo;
            var opponentsInfo = sessionPlay.OpponentPlayerInfo;

            // Prepare the round result for the current player
            var currentsRoundResult = new RoundResult
            {
                OpponentsTotalScore = opponentsInfo.Score,
                YourTotalScore = currentInfo.Score,
                YourRemainingPlayCount = _maximumPlayCount - currentInfo.PlayCount,
                OpponentsRemainingPlayCount = _maximumPlayCount - opponentsInfo.PlayCount,
                IsYourTurn = false, // It’s no longer the current player's turn
                DiceResult = diceResult,
            };

            // Prepare the round result for the opponent player
            var opponentsRoundResult = new RoundResult
            {
                OpponentsTotalScore = currentInfo.Score,
                YourTotalScore = opponentsInfo.Score,
                YourRemainingPlayCount = _maximumPlayCount - opponentsInfo.PlayCount,
                OpponentsRemainingPlayCount = _maximumPlayCount - currentInfo.PlayCount,
                IsYourTurn = true, // It’s now the opponent player's turn
                DiceResult = diceResult,
            };

            // Send round result updates to both players
            currentInfo.Player.User.Client.RoundResult(currentsRoundResult);
            opponentsInfo.Player.User.Client.RoundResult(opponentsRoundResult);

            // Switch the turn to the next player
            sessionPlay.SwitchPlayers();
        }

        public async Task RemovePlayer(IUser user)
        {
            // Find the session that contains the play for the user
            var session = _sessions.Values
                .SingleOrDefault(x => x.Plays.Any(p => p.CurrentPlayerInfo.UserId == user.Id));

            // Exit if the session is not found
            if (session is null) return;

            // Find the ongoing play involving the user
            var sessionPlay = session.Plays.SingleOrDefault(play => !play.IsFinished
                && (play.CurrentPlayerInfo.UserId == user.Id || play.OpponentPlayerInfo.UserId == user.Id))
                ?? throw new Exception("Play is null");

            // Determine the ID of the other player in the play
            var otherPlayerId = sessionPlay.CurrentPlayerInfo.UserId == user.Id
                ? sessionPlay.OpponentPlayerInfo.UserId
                : sessionPlay.CurrentPlayerInfo.UserId;

            // End the game, declaring the other player as the winner
            await EndGame(sessionPlay.Id, otherPlayerId);
        }
    }
}
