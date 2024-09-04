using DataTransferModels.Requests;
using Microsoft.AspNetCore.SignalR;
using DataTransferModels.Clients;
using ApplicationTemplate.Server.Common.Interfaces;
using ApplicationTemplate.Server.Common.Security;
using ApplicationTemplate.Server.Commands;
using Domain.Models;
using System.Collections.Generic;
using Web.Services;
using Web.Services.IServices;
using ApplicationTemplate.Server.Services;
using DataTransferModels.ValueObjects;
using System.Runtime.CompilerServices;
using static Web.Helpers.HttpContextExtensions;

namespace Web.Hubs
{
    // This Hub handles the real-time communication between the server and clients for the game.
    [Authorize]
    public class ApplicationHub(IMediatorService mediator, IPlayerManager playerManager, ILogger<ApplicationHub> logger) : Hub<IApplicationClient>, IApplicationHub
    {
        private readonly IMediatorService _mediator = mediator;
        private readonly IPlayerManager _playerManager = playerManager;
        private readonly ILogger<ApplicationHub> _logger = logger;

        // This method is called when a client successfully connects to the Hub.
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("OnConnectedAsync");

            // Create a user object using the current connection context and the calling client.
            var user = new CurrentUser(Context, Clients.Caller);

            _logger.LogInformation($"User: {user.Id}");

            // Send a connect command through the mediator service, initiating any necessary connection logic.
            await _mediator.ConnectAsync(new ConnectCommand(user));

            await base.OnConnectedAsync();
        }

        // Property to retrieve the connected user based on the current HTTP context.
        private IUser ConnectedUser
        {
            get
            {
                _logger.LogInformation("Getting ConnectedUser");

                // Extract the userId from the HTTP context.
                var userId = Context.GetHttpContext()!.GetUserIdFromHttpContext();

                _logger.LogInformation($"userId: {userId}");

                // Fetch the game player corresponding to the userId.
                var gamePlayer = _playerManager.GetPlayer(userId).Result;

                _logger.LogInformation($"gamePlayer: {gamePlayer.User.Id}");

                return gamePlayer.User;
            }
        }

        // This method is called when a client disconnects from the Hub.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Send a disconnect command through the mediator service, performing any necessary cleanup.
            await _mediator.DisconnectAsync(new DisconnectCommand(ConnectedUser, exception));
            await base.OnDisconnectedAsync(exception);
        }

        // Method for joining a room, invoked by clients.
        public async Task JoinRoomAsync()
        {
            await _mediator.JoinRoomAsync(new JoinRoomCommand(ConnectedUser));
        }

        // Method for requesting a rematch in an existing session, invoked by clients.
        public async Task RematchAsync(long sessionId)
        {
            await _mediator.RematchAsync(new RematchCommand(sessionId, ConnectedUser));
        }

        // Method for canceling a rematch request, invoked by clients.
        public async Task CancelRematchAsync(long sessionId)
        {
            await _mediator.CancelRematchAsync(new CancelRematchCommand(sessionId, ConnectedUser));
        }

        // Method for leaving a room, invoked by clients.
        public async Task LeaveRoomAsync(long sessionId)
        {
            await _mediator.LeaveRoomAsync(new LeaveRoomCommand(ConnectedUser, sessionId));
        }

        // Method for indicating that a player is ready to start the game, invoked by clients.
        public async Task ReadyAsync()
        {
            await _mediator.ReadyAsync(new ReadyCommand(ConnectedUser));
        }

        // Method for indicating that a player is not ready, invoked by clients.
        public async Task NotReadyAsync()
        {
            await _mediator.NotReadyAsync(new NotReadyCommand(ConnectedUser));
        }

        // Method for processing a turn during the game, invoked by clients.
        public async Task ProccessTurnAsync(ProccessTurnRequest request)
        {
            var command = new ProccessTurnCommand(ConnectedUser, request.SessionId, request.SessionPlayId, request.DiceSetType);

            await _mediator.ProccessTurnAsync(command);
        }
    }
}
