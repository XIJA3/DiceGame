using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static ApplicationTemplate.Server.Services.DiceService;

namespace ApplicationTemplate.Server.Services.IServices
{
    public interface ISessionService
    {
        Task<GameSession> StartSession(GameRoom gameRoom);
        Task EndSession(long sessionId, IUser loser);
        Task<GameSessionPlay> GetCurrentSessionPlay(long sessionId, long sessionPlayId);
        Task<GameSession?> GetSessionAsync(long sessionId);
        Task ProcessTurnAsync(long sessionId, long sessionPlayId, IUser user, DiceSetTypes diceSetType);
        Task RemovePlayer(IUser user);
        Task EndGame(long sessionPlayId, long? winnerId = null);
        Task StartGame(long sessionId);
    }
}
