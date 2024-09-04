using ApplicationTemplate.Server.Models;
using DataTransferModels.Requests;
using DataTransferModels.Responses;
using DataTransferModels.ValueObjects;
using Enums.Enums;
using Game.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace Game.Pages
{
    public class MainPage : ComponentBase
    {
        // Game-related properties
        protected GameResults YourResult { get; private set; }
        protected GameState State = GameState.Idle;
        protected PlayerRoomStatus YourRoomStatus = PlayerRoomStatus.NotReady;
        protected PlayerRoomStatus OpponentsRoomStatus = PlayerRoomStatus.NotReady;
        private long SessionId { get; set; }
        private long SessionPlayId { get; set; }
        protected int OnlinePlayerCount { get; set; }

        protected int CurrentDiceResult { get; private set; }
        protected string OpponentsName { get; private set; } = string.Empty;
        protected long OpponentsTotalScore { get; private set; }
        protected long OpponentsRemainingPlayCount { get; private set; }

        protected string YourName { get; private set; } = string.Empty;
        protected long YourRemainingPlayCount { get; private set; }
        protected long YourTotalScore { get; private set; }
        protected bool IsYourTurn { get; private set; }
        protected bool OpponentDisconnected { get; private set; }

        // Services and navigation components
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private IAuthService _authService { get; set; } = null!;
        [Inject] private IGameHubService _gameHub { get; set; } = null!;
        [Inject] private ISnackbar _snackbar { get; set; } = null!;
        [Inject] private NavigationManager _navManager { get; set; } = null!;

        // Method that runs after the component has rendered
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await CheckStartingPoint();

            await base.OnAfterRenderAsync(firstRender);
        }

        // Checks if the user is authorized and connects to the SignalR hub
        private async Task CheckStartingPoint()
        {
            if (await _authService.IsAuthorized())
            {
                YourName = (await _authService.GetUserName())!;

                await ConnectToHub();
            }
            else
            {
                _navManager.NavigateTo("/Login");
            }

            StateHasChanged();
        }

        // Attempts to connect to the SignalR hub with retry logic
        private async Task ConnectToHub()
        {
            try
            {
                int tryCount = 0;

                // Retry logic for connecting to the hub
                while (_gameHub.ConnectionState != HubConnectionState.Connected && tryCount <= 3)
                {
                    SetState(GameState.Connecting);

                    await _gameHub.ConnectToHubAsync();

                    await Task.Delay(1000);

                    tryCount++;
                }

                if (_gameHub.ConnectionState != HubConnectionState.Connected)
                {
                    _snackbar.Add("Could Not Connect To Hub", Severity.Error);

                    await _authService.LogoutAsync();

                    _navManager.NavigateTo("/Login");
                }
                else
                {
                    await RegisterEventHandlers();
                }

                SetState(GameState.Idle);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("ERROR HUB" + ex.Message + "INNER ERROR HUB" + ex.InnerException?.Message);
            }
        }

        // Resets game and player state to initial values
        private void StartOver()
        {
            YourRoomStatus = PlayerRoomStatus.NotReady;
            OpponentsRoomStatus = PlayerRoomStatus.NotReady;

            SessionId = 0;
            SessionPlayId = 0;

            CurrentDiceResult = 0;

            YourTotalScore = 0;
            OpponentsTotalScore = 0;

            YourRemainingPlayCount = 0;
            OpponentsRemainingPlayCount = 0;

            OpponentsName = string.Empty;
            IsYourTurn = false;
            OpponentDisconnected = false;

            SetState(GameState.Idle);
        }

        // Logs out the user and navigates to the login page
        protected async Task LogOut()
        {
            await _authService.LogoutAsync();

            _navManager.NavigateTo("/login");
        }

        // Navigates to the leaderboard page
        protected void NavigateToLeaderboard()
        {
            _navManager.NavigateTo("/leaderboard");
        }

        // Opens a dialog with the game rules
        protected void OpenGameRulesDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            DialogService.ShowAsync<GameRulesDialog>("Game Rules", options);
        }

        // Requests to leave the current room
        public async Task Leave()
        {
            await _gameHub.LeaveRoomAsync(SessionId);
        }

        // Joins a room
        public void JoinRoom()
        {
            _gameHub.JoinRoomAsync();
        }

        // Requests a rematch or cancels the rematch based on the current status
        public void Rematch()
        {
            if (YourRoomStatus == PlayerRoomStatus.Ready)
                _gameHub.CancelRematchAsync(SessionId);
            else
                _gameHub.RematchAsync(SessionId);
        }

        // Changes the player's ready status
        public void Ready()
        {
            if (YourRoomStatus == PlayerRoomStatus.Ready)
                _gameHub.NotReadyAsync();
            else
                _gameHub.ReadyAsync();
        }

        // Rolls the dice and processes the turn
        public async Task RollDice(DiceSetTypes diceSetTypes)
        {
            var request = new ProccessTurnRequest(SessionId, SessionPlayId, diceSetTypes);

            await _gameHub.ProccessTurnAsync(request);
        }

        // Registers various event handlers for the game hub
        private async Task RegisterEventHandlers()
        {
            await _gameHub.RegisterOnYourRoomStatus((status) =>
            {
                YourRoomStatus = status;

                StateHasChanged();
            });

            await _gameHub.RegisterYouLeftRoom(() =>
            {
                SetState(GameState.Idle);
            });

            await _gameHub.RegisterWaitingOpponent(() =>
            {
                SetState(GameState.SearchingOpponent);
            });

            await _gameHub.RegisterOnOnlinePlayerCount((count) =>
            {
                OnlinePlayerCount = count;

                StateHasChanged();
            });

            await _gameHub.RegisterGameStarted((response) =>
            {
                StartOver();

                SessionId = response.SessionId;
                SessionPlayId = response.SessionPlayId;
                OpponentsName = response.OpponentsName;
                IsYourTurn = response.IsYourTurn;
                YourRemainingPlayCount = response.MaximumPlayCount;
                OpponentsRemainingPlayCount = response.MaximumPlayCount;

                SetState(GameState.StartedMatchmaking);
            });

            await _gameHub.RegisterRoundResult((response) =>
            {
                YourTotalScore = response.YourTotalScore;
                OpponentsTotalScore = response.OpponentsTotalScore;
                CurrentDiceResult = response.DiceResult;
                IsYourTurn = response.IsYourTurn;
                YourRemainingPlayCount = response.YourRemainingPlayCount;
                OpponentsRemainingPlayCount = response.OpponentsRemainingPlayCount;

                SetState(GameState.InProgress);
            });

            await _gameHub.RegisterStartedMatchMaking(() =>
            {
                SetState(GameState.JoinedInTheRoom);
            });

            await _gameHub.RegisterYourGameResult((gameResult) =>
            {
                YourTotalScore = gameResult.YourTotalScore;
                OpponentsTotalScore = gameResult.OpponentsTotalScore;
                YourResult = gameResult.YourGameResult;

                SetState(GameState.GameOver);
            });

            // Handling different game states and events
            await _gameHub.RegisterYouLeftRoom(() =>
            {
                SetMessageAsync("You Left Room");

                StartOver();
            });

            await _gameHub.RegisterPlayerNotFound(() =>
            {
                SetMessageAsync("Player Not Found");

                StartOver();
            });

            await _gameHub.RegisterOpponentDisconnected(() =>
            {
                SetMessageAsync("Opponent Has Leave Game");

                OpponentDisconnected = true;

                if (State == GameState.StartedMatchmaking)
                    SetState(GameState.GameOver);
                else
                    StartOver();
            });

            await _gameHub.RegisterGameIsOpenInOtherWindow(async () =>
            {
                await SetMessageAsync("Game Is Open In Other Window");

                StartOver();

                await LogOut();
            });

            await _gameHub.RegisterOnOpponentsRoomStatus((status) =>
            {
                OpponentsRoomStatus = status;

                SetMessageAsync($"Opponent is {status}");
            });
        }

        // Displays a message to the user
        public async Task SetMessageAsync(string message)
        {
            _snackbar.Add(message, Severity.Info);
        }

        // Updates the component state and triggers a re-render
        private void SetState(GameState state)
        {
            State = state;

            StateHasChanged();
        }

        // Enumeration for the different game states
        public enum GameState
        {
            Connecting,
            Idle,
            WaitingRematch,
            SearchingOpponent,
            JoinedInTheRoom,
            StartedMatchmaking,
            InProgress,
            YourTurn,
            OpponentsTurn,
            GameOver,
        }
    }
}
