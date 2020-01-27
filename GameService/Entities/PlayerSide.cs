using System;
using System.Collections.Generic;
using System.Linq;

namespace GameService
{
    internal class CPlayerSide
    {
        public Dictionary<Int32, CVirtualPiece> MovingPieces { get; }

        public Dictionary<Int32, CVirtualPiece> AllPieces { get; }

        public HashSet<Int32> DelayRemoveStoppedPiecesKeys { get; }

        public HashSet<Int32> DelayRemovePiecesKeys { get; }

        public HashSet<Int32> PawnsKeys { get; }

        public CPlayer Player { get; }

        public CPlayerSide(CPlayer player, Dictionary<Int32, CVirtualPiece> allPieces, HashSet<Int32> pawnsKeys)
        {
            Player = player;
            AllPieces = allPieces;
            MovingPieces = new Dictionary<Int32, CVirtualPiece>();
            PawnsKeys = pawnsKeys;
            DelayRemovePiecesKeys = new HashSet<Int32>();
            DelayRemoveStoppedPiecesKeys = new HashSet<Int32>();
        }

        public void RemoveCapturedPieces()
        {
            foreach (Int32 pieceKey in DelayRemovePiecesKeys)
            {
                if (PawnsKeys.Contains(pieceKey))
                    PawnsKeys.Remove(pieceKey);
                if (MovingPieces.ContainsKey(pieceKey))
                    MovingPieces.Remove(pieceKey);
                AllPieces.Remove(pieceKey);
            }
            DelayRemovePiecesKeys.Clear();
        }

        public void RemoveStoppedPieces()
        {
            foreach (Int32 pieceKey in DelayRemoveStoppedPiecesKeys.Where(pieceKey => MovingPieces.ContainsKey(pieceKey)))
            {
                MovingPieces.Remove(pieceKey);
            }
            DelayRemoveStoppedPiecesKeys.Clear();
        }
    }
}
