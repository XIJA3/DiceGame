using ApplicationTemplate.Server.Helpers;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Models
{
    public class GameSessionPlay
    {
        public long Id { get; }
        private PlayerInfo _player1 = null!;
        private PlayerInfo _player2 = null!;
        public bool IsFinished { get; set; }
        public PlayerInfo CurrentPlayerInfo => _player1.IsCurrentPlayer ? _player1 : _player2;
        public PlayerInfo OpponentPlayerInfo => _player1.IsCurrentPlayer ? _player2 : _player1;


        public GameSessionPlay(long sessionId, GamePlayer gamePlayer1, GamePlayer gamePlayer2)
        {
            Id = sessionId;
            InitializePlayers(gamePlayer1, gamePlayer2);
        }


        private void InitializePlayers(GamePlayer gamePlayer1, GamePlayer gamePlayer2)
        {
            var randomCurrentPlayer = RandomExtensions.GetRandom(gamePlayer1, gamePlayer2);
            _player1 = new PlayerInfo(gamePlayer1, randomCurrentPlayer == gamePlayer1);
            _player2 = new PlayerInfo(gamePlayer2, randomCurrentPlayer == gamePlayer2);
        }

        public void SwitchPlayers()
        {
            lock (this)
            {
                _player1.IsCurrentPlayer = !_player1.IsCurrentPlayer;
                _player2.IsCurrentPlayer = !_player2.IsCurrentPlayer;
            }
        }


        public class PlayerInfo(GamePlayer player, bool isCurrentPlayer = false)
        {
            public long UserId { get; private set; } = player.User.Id;
            public long Score { get; set; }
            public long PlayCount { get; set; }
            public bool IsCurrentPlayer { get; set; } = isCurrentPlayer;
            public GamePlayer Player { get; set; } = player;
        }
    }
}
