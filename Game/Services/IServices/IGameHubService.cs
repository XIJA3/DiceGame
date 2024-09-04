using ApplicationTemplate.Server.Models;
using Client.Services.IServices;
using DataTransferModels.Clients;
using DataTransferModels.Responses;
using DataTransferModels.ValueObjects;
using Enums.Enums;
using System.Linq.Expressions;

namespace Game.Services.IServices
{
    public interface IGameHubService : IBaseHubService, IApplicationHub
    {
        Task<IDisposable> RegisterGameIsOpenInOtherWindow(Action action);
        Task<IDisposable> RegisterWaitingOpponent(Action action);
        Task<IDisposable> RegisterStartedMatchMaking(Action action);
        Task<IDisposable> RegisterAlreadyInRoom(Action action);
        Task<IDisposable> RegisterYouLeftRoom(Action action);
        Task<IDisposable> RegisterOnYourRoomStatus(Action<PlayerRoomStatus> action);
        Task<IDisposable> RegisterOnOpponentsRoomStatus(Action<PlayerRoomStatus> action);
        Task<IDisposable> RegisterOnOnlinePlayerCount(Action<int> action);
        Task<IDisposable> RegisterPlayerNotFound(Action action);
        Task<IDisposable> RegisterOpponentDisconnected(Action action);
        Task<IDisposable> RegisterNotYourTurn(Action action);
        Task<IDisposable> RegisterGameStarted(Action<GameStartResponse> action);
        Task<IDisposable> RegisterRoundResult(Action<RoundResult> action);
        Task<IDisposable> RegisterYourGameResult(Action<GameFinishResult> action);
    }
}
