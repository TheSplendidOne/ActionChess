using System;

namespace ActionChessClient
{
    internal class CGameStartParameters
    {
        public Guid GameId { get; }

        public Guid OpponentId { get; }

        public EPieceColor MySideColor { get; }

        public CGameStartParameters(Guid gameId, Guid opponentId, EPieceColor mySideColor)
        {
            GameId = gameId;
            OpponentId = opponentId;
            MySideColor = mySideColor;
        }
    }
}
