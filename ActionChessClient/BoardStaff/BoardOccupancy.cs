using System;

namespace ActionChessClient
{
    internal class CBoardOccupancy
    {
        private readonly EPieceColor[,] _states;

        public EPieceColor this[Int32 x, Int32 y]
        {
            get => _states[y, x];
            set => _states[y, x] = value;
        }

        public EPieceColor this[CTile tile]
        {
            get => _states[tile.Y, tile.X];
            set => _states[tile.Y, tile.X] = value;
        }

        public CBoardOccupancy() : this(new []
        {
            "bbbbbbbb",
            "bbbbbbbb",
            "........",
            "........",
            "........",
            "........",
            "wwwwwwww",
            "wwwwwwww"
        })
        {
        }

        public CBoardOccupancy(String[] board)
        {
            static EPieceColor GetPieceColor(Char symbol)
            {
                if (symbol == 'w')
                    return EPieceColor.White;
                if (symbol == 'b')
                    return EPieceColor.Black;
                return EPieceColor.None;
            }
            _states = new EPieceColor[8, 8];
            for(Int32 y = 0; y < 8; y++)
            for (Int32 x = 0; x < 8; x++)
                _states[y, x] = GetPieceColor(board[y][x]);
        }

        public void ResetToDefault(CTile tile)
        {
            this[tile] = EPieceColor.None;
        }
    }
}