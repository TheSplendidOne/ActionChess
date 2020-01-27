using System;
using System.Windows;

namespace ActionChessClient
{
    internal class CTile
    {
        public Int32 X { get; set; }

        public Int32 Y { get; set; }

        public CTile(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }

        public CTile(Thickness thickness)
        {
            X = (Int32) thickness.Left / CPiece.PieceSize;
            Y = (Int32) thickness.Top / CPiece.PieceSize;
        }

        public CTile(Point point)
        {
            X = (Int32) point.X / CPiece.PieceSize;
            Y = (Int32) point.Y / CPiece.PieceSize;
        }

        public CTile(CTile tile)
        {
            X = tile.X;
            Y = tile.Y;
        }

        public override Boolean Equals(Object obj)
        {
            if (!(obj is CTile))
                return false;
            return X == ((CTile) obj).X && Y == ((CTile) obj).Y;
        }

        public override Int32 GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}