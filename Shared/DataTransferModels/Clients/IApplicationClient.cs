using ApplicationTemplate.Server.Models;
using DataTransferModels.Responses;
using Enums.Enums;

namespace DataTransferModels.Clients
{
    public interface IApplicationClient
    {
        // General
        Task GameIsOpenInOtherWindow();
        Task OnlinePlayerCount(int playerCount);

        // Matchmaking
        Task WaitingOpponent();
        Task StartedMatchMaking();
        Task AlreadyInRoom();
        Task YourRoomStatus(PlayerRoomStatus status);
        Task OpponentsRoomStatus(PlayerRoomStatus status);
        Task YouLeftRoom();
        Task PlayerNotFound();
        Task OpponentDisconnected();

        // Game
        Task NotYourTurn();
        Task GameStarted(GameStartResponse gameStartResponse);
        Task RoundResult(RoundResult userData);
        Task YourGameResult(GameFinishResult gameFinishResult);
    }
}