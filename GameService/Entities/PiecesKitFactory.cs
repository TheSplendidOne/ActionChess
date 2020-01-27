using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameService
{
    internal static class SPiecesKitFactory
    {
        public static readonly Int32 WhiteKingId = 13;

        public static readonly Int32 BlackKingId = 29;

        public static Dictionary<Int32, CVirtualPiece> GetPiecesStartKit(ESideColor color)
        {
            return color switch
            {
                ESideColor.White => GetWhitePiecesStartKit(),
                ESideColor.Black => GetBlackPiecesStartKit(),
                _ => throw new InvalidEnumArgumentException($"Unknown kit color: {color}")
            };
        }

        private static Dictionary<Int32, CVirtualPiece> GetWhitePiecesStartKit()
        {
            Dictionary<Int32, CVirtualPiece> dictionary = new Dictionary<Int32, CVirtualPiece>();
            for (Int32 i = 0; i < 8; i++)
            {
                dictionary.Add(i + 1 + 0, new CVirtualPiece(i, 6));
                dictionary.Add(i + 1 + 8, new CVirtualPiece(i, 7));
            }
            return dictionary;
        }

        private static Dictionary<Int32, CVirtualPiece> GetBlackPiecesStartKit()
        {
            Dictionary<Int32, CVirtualPiece> dictionary = new Dictionary<Int32, CVirtualPiece>();
            for (Int32 i = 0; i < 8; i++)
            {
                dictionary.Add(i + 1 + 24, new CVirtualPiece(i, 0));
                dictionary.Add(i + 1 + 16, new CVirtualPiece(i, 1));
            }
            return dictionary;
        }

        public static HashSet<Int32> GetPawns(ESideColor color)
        {
            return color switch
            {
                ESideColor.White => GetPawns(1),
                ESideColor.Black => GetPawns(17),
                _ => throw new InvalidEnumArgumentException($"Unknown kit color: {color}")
            };
        }

        private static HashSet<Int32> GetPawns(Int32 smallestPawnId)
        {
            HashSet<Int32> set = new HashSet<Int32>();
            for (Int32 i = smallestPawnId; i < smallestPawnId + 8; i++)
                set.Add(i);
            return set;
        }
    }
}
