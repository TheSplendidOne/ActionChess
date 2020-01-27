using System;

namespace ActionChessClient
{
    internal class CGameStartModel
    {
        public Guid GameId { get; }

        public Guid OpponentId { get; }

        public EPieceColor MySideColor { get; }

        public CGameStartModel(Guid gameId, Guid opponentId, EPieceColor mySideColor)
        {
            GameId = gameId;
            OpponentId = opponentId;
            MySideColor = mySideColor;
        }
    }
}
