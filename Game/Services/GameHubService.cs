using ApplicationTemplate.Server.Models;
using Client.Services;
using DataTransferModels.Clients;
using DataTransferModels.Requests;
using DataTransferModels.Responses;
using DataTransferModels.ValueObjects;
using Enums.Enums;
using Game.Helpers;
using Game.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Linq.Expressions;

namespace Game.Services
{
    /// <summary>
    /// Provides game-specific functionalities for interacting with the SignalR Hub.
    /// </summary>
    public class GameHubService(IAuthService authManager, IConfiguration configuration, NavigationManager navigationManager)
               : BaseHubService(authManager, configuration, navigationManager), IGameHubService
    {
        // Invokes the JoinRoomAsync method on the Hub
        public async Task JoinRoomAsync()
        {
            await HubConnection.InvokeAsync(nameof(IApplicationHub.JoinRoomAsync));
        }

        // Invokes the LeaveRoomAsync method on the Hub with a session ID
        public async Task LeaveRoomAsync(long sessionId)
        {
            await HubConnection.InvokeAsync(nameof(IApplicationHub.LeaveRoomAsync), sessionId);
        }

        // Invokes the RematchAsync method on the Hub with a session ID
        public async Task RematchAsync(long sessionId)
        {
            await HubConnection.InvokeAsync<long>(nameof(IApplicationHub.RematchAsync), sessionId);
        }

        // Invokes the CancelRematchAsync method on the Hub with a session ID
        public async Task CancelRematchAsync(long sessionId)
        {
            await HubConnection.InvokeAsync<long>(nameof(IApplicationHub.CancelRematchAsync), sessionId);
        }

        // Invokes the ReadyAsync method on the Hub
        public async Task ReadyAsync()
        {
            await HubConnection.InvokeAsync(nameof(IApplicationHub.ReadyAsync));
        }

        // Invokes the NotReadyAsync method on the Hub
        public async Task NotReadyAsync()
        {
            await HubConnection.InvokeAsync(nameof(IApplicationHub.NotReadyAsync));
        }

        // Invokes the ProccessTurnAsync method on the Hub with a ProccessTurnRequest
        public async Task ProccessTurnAsync(ProccessTurnRequest request)
        {
            await HubConnection.InvokeAsync(nameof(IApplicationHub.ProccessTurnAsync), request);
        }

        // Registers an action to be called when the GameIsOpenInOtherWindow event occurs
        public async Task<IDisposable> RegisterGameIsOpenInOtherWindow(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.GameIsOpenInOtherWindow), action);
        }

        // Registers an action to be called when the WaitingOpponent event occurs
        public async Task<IDisposable> RegisterWaitingOpponent(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.WaitingOpponent), action);
        }

        // Registers an action to be called when the StartedMatchMaking event occurs
        public async Task<IDisposable> RegisterStartedMatchMaking(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.StartedMatchMaking), action);
        }

        // Registers an action to be called when the AlreadyInRoom event occurs
        public async Task<IDisposable> RegisterAlreadyInRoom(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.AlreadyInRoom), action);
        }

        // Registers an action to be called when the YouLeftRoom event occurs
        public async Task<IDisposable> RegisterYouLeftRoom(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.YouLeftRoom), action);
        }

        // Registers an action to be called when the PlayerNotFound event occurs
        public async Task<IDisposable> RegisterPlayerNotFound(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.PlayerNotFound), action);
        }

        // Registers an action to be called when the OpponentDisconnected event occurs
        public async Task<IDisposable> RegisterOpponentDisconnected(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.OpponentDisconnected), action);
        }

        // Registers an action to be called when the NotYourTurn event occurs
        public async Task<IDisposable> RegisterNotYourTurn(Action action)
        {
            return HubConnection.On(nameof(IApplicationClient.NotYourTurn), action);
        }

        // Registers an action to be called when the GameStarted event occurs
        public async Task<IDisposable> RegisterGameStarted(Action<GameStartResponse> action)
        {
            return HubConnection.On(nameof(IApplicationClient.GameStarted), action);
        }

        // Registers an action to be called when the RoundResult event occurs
        public async Task<IDisposable> RegisterRoundResult(Action<RoundResult> action)
        {
            return HubConnection.On(nameof(IApplicationClient.RoundResult), action);
        }

        // Registers an action to be called when the YourGameResult event occurs
        public async Task<IDisposable> RegisterYourGameResult(Action<GameFinishResult> action)
        {
            return HubConnection.On(nameof(IApplicationClient.YourGameResult), action);
        }

        // Registers an action to be called when the YourRoomStatus event occurs
        public async Task<IDisposable> RegisterOnYourRoomStatus(Action<PlayerRoomStatus> action)
        {
            return HubConnection.On(nameof(IApplicationClient.YourRoomStatus), action);
        }

        // Registers an action to be called when the OpponentsRoomStatus event occurs
        public async Task<IDisposable> RegisterOnOpponentsRoomStatus(Action<PlayerRoomStatus> action)
        {
            return HubConnection.On(nameof(IApplicationClient.OpponentsRoomStatus), action);
        }

        // Registers an action to be called when the OnlinePlayerCount event occurs
        public async Task<IDisposable> RegisterOnOnlinePlayerCount(Action<int> action)
        {
            return HubConnection.On(nameof(IApplicationClient.OnlinePlayerCount), action);
        }

        // Registers an action to be called when the OnlinePlayers event occurs
        public async Task<IDisposable> RegisterOnOnlinePlayers(Action<List<int>> action)
        {
            return HubConnection.On(nameof(IApplicationClient.OnlinePlayerCount), action);
        }
    }
}
