using System;
using System.ComponentModel;
using System.Windows.Controls;
using NUnit.Framework;

namespace ActionChessClient
{
    internal static class SMovementValidatorFactory
    {
        public static CMovementValidator GetMovementValidator(EPieceType type)
        {
            return (type switch
            {
                EPieceType.Pawn => PawnMovementValidator,
                EPieceType.Knight => KnightMovementValidator,
                EPieceType.Bishop => BishopMovementValidator,
                EPieceType.Rook => RookMovementValidator,
                EPieceType.Queen => QueenMovementValidator,
                EPieceType.King => KingMovementValidator,
                _ => throw new InvalidEnumArgumentException($"Unknown piece type: {type}")
            });
        }

        #region MovementValidators

        public static CMovementValidator PawnMovementValidator =>
            new CMovementValidator(PawnOneTiledVerticalMovement, 
                PawnTwoTiledVerticalMovement,
                PawnDiagonalMovement);

        public static  CMovementValidator KnightMovementValidator =>
            new CMovementValidator(KnightMovement);

        public static CMovementValidator BishopMovementValidator =>
            new CMovementValidator(DiagonalMovement);

        public static CMovementValidator RookMovementValidator =>
            new CMovementValidator(VerticalMovement, HorizontalMovement);

        public static CMovementValidator QueenMovementValidator =>
            new CMovementValidator(VerticalMovement,
                HorizontalMovement,
                DiagonalMovement);

        public static CMovementValidator KingMovementValidator =>
            new CMovementValidator(KingMovement);

        #endregion

        #region MovementRules

        private static readonly MovementRule PawnTwoTiledVerticalMovement =
            (states, currentTile, proposedTile) =>
                IsMovementVertical(currentTile, proposedTile) &&
                IsSpaceBetweenTilesFree(states, currentTile, proposedTile) &&
                !IsProposedTileYours(states[currentTile], states[proposedTile]) &&
                IsPawnNeverMovingBefore(states[currentTile], currentTile) &&
                new CTileVector(currentTile, proposedTile).YProjection == 2 * PawnColorDirectionModifier(states[currentTile]);

        private static readonly MovementRule PawnOneTiledVerticalMovement =
            (states, currentTile, proposedTile) =>
                IsMovementVertical(currentTile, proposedTile) &&
                IsProposedTileNobody(states[proposedTile]) &&
                new CTileVector(currentTile, proposedTile).YProjection == 1 * PawnColorDirectionModifier(states[currentTile]);

        private static readonly MovementRule PawnDiagonalMovement =
            (states, currentTile, proposedTile) =>
                IsProposedTileOpponent(states[currentTile], states[proposedTile]) &&
                Math.Abs(new CTileVector(currentTile, proposedTile).XProjection) == 1 &&
                new CTileVector(currentTile, proposedTile).YProjection == 1 * PawnColorDirectionModifier(states[currentTile]);

        private static readonly MovementRule KnightMovement =
            (states, currentTile, proposedTile) =>
            {
                CTileVector vector = new CTileVector(currentTile, proposedTile);
                return (Math.Abs(vector.XProjection) == 1 &&
                        Math.Abs(vector.YProjection) == 2 ||
                        Math.Abs(vector.XProjection) == 2 &&
                        Math.Abs(vector.YProjection) == 1) &&
                       !IsProposedTileYours(states[currentTile], states[proposedTile]);
            };

        private static readonly MovementRule HorizontalMovement =
            (states, currentTile, proposedTile) =>
                IsMovementHorizontal(currentTile, proposedTile) &&
                IsSpaceBetweenTilesFree(states, currentTile, proposedTile) &&
                !IsProposedTileYours(states[currentTile], states[proposedTile]);

        private static readonly MovementRule VerticalMovement =
            (states, currentTile, proposedTile) =>
                IsMovementVertical(currentTile, proposedTile) &&
                IsSpaceBetweenTilesFree(states, currentTile, proposedTile) &&
                !IsProposedTileYours(states[currentTile], states[proposedTile]);

        private static readonly MovementRule DiagonalMovement =
            (states, currentTile, proposedTile) =>
                IsMovementDiagonal(currentTile, proposedTile) &&
                IsSpaceBetweenTilesFree(states, currentTile, proposedTile) &&
                !IsProposedTileYours(states[currentTile], states[proposedTile]);

        private static readonly MovementRule KingMovement =
            (states, currentTile, proposedTile) =>
            {
                CTileVector vector = new CTileVector(currentTile, proposedTile);
                return Math.Abs(vector.XProjection) <= 1 &&
                       Math.Abs(vector.YProjection) <= 1 &&
                       !IsProposedTileYours(states[currentTile], states[proposedTile]);
            };

        #endregion

        #region RuleParts

        private static Int32 PawnColorDirectionModifier(EPieceColor pawnColor)
        {
            return pawnColor switch
            {
                EPieceColor.White => -1,
                EPieceColor.Black => 1,
                _ => throw new ArgumentException($"Unknown or undefined piece color: {pawnColor}")
            };
        }

        private static Boolean IsSpaceBetweenTilesFree(CBoardOccupancy boardStates, CTile tileA, CTile tileB)
        {
            if (!IsMovementDiagonal(tileA, tileB) && !IsMovementStraight(tileA, tileB))
                return false;
            Action<CTile> goNextTile;
            CTile goingTile = new CTile(tileA);
            CTileVector vector = new CTileVector(tileA, tileB);
            if (vector.XProjection > 0)
                goNextTile = tile => ++tile.X;
            else if (vector.XProjection < 0)
                goNextTile = tile => --tile.X;
            else
                goNextTile = tile => { };
            if (vector.YProjection > 0)
                goNextTile += tile => ++tile.Y;
            else if (vector.YProjection < 0)
                goNextTile += tile => --tile.Y;
            while (true)
            {
                goNextTile(goingTile);
                if (IsTheSameTile(goingTile, tileB))
                    return true;
                if (boardStates[goingTile] != EPieceColor.None)
                    return false;
            }
        }

        private static Boolean IsMovementDiagonal(CTile tileA, CTile tileB)
        {
            CTileVector tileVector = new CTileVector(tileA, tileB);
            return Math.Abs(tileVector.XProjection) == Math.Abs(tileVector.YProjection);
        }

        private static Boolean IsMovementHorizontal(CTile tileA, CTile tileB)
        {
            return new CTileVector(tileA, tileB).YProjection == 0;
        }

        private static Boolean IsMovementVertical(CTile tileA, CTile tileB)
        {
            return new CTileVector(tileA, tileB).XProjection == 0;
        }

        private static Boolean IsMovementStraight(CTile tileA, CTile tileB)
        {
            return IsMovementHorizontal(tileA, tileB) || IsMovementVertical(tileA, tileB);
        }

        private static Boolean IsProposedTileNobody(EPieceColor proposedTileColor)
        {
            return proposedTileColor == EPieceColor.None;
        }

        private static Boolean IsProposedTileYours(EPieceColor currentTileColor, EPieceColor proposedTileColor)
        {
            return currentTileColor == proposedTileColor;
        }

        private static Boolean IsProposedTileOpponent(EPieceColor currentTileColor, EPieceColor proposedTileColor)
        {
            return currentTileColor switch
            {
                EPieceColor.White => (proposedTileColor == EPieceColor.Black),
                EPieceColor.Black => (proposedTileColor == EPieceColor.White),
                _ => throw new ArgumentException($"Unknown or undefined color: {currentTileColor}")
            };
        }

        private static Boolean IsTheSameTile(CTile tileA, CTile tileB)
        {
            return tileA.Equals(tileB);
        }

        private static Boolean IsPawnNeverMovingBefore(EPieceColor pawnColor, CTile currentTile)
        {
            return pawnColor switch
            {
                EPieceColor.White => (currentTile.Y == 6), // 6 - start Y component for white pawn
                EPieceColor.Black => (currentTile.Y == 1), // 1 - start Y component for black pawn
                _ => throw new ArgumentException("Current tile nobody is busy")
            };
        }

        #endregion

        #region Tests

        [TestFixture]
        private class Tests
        {
            [Test]
            public void Test1()
            {
                CBoardOccupancy occupancy = new CBoardOccupancy(new[]{
                    "bbbbbbbb",
                    "bb.bbbbb",
                    "........",
                    "..b.....",
                    "........",
                    "........",
                    "wwwwwwww",
                    "wwwwwwww"});
                CTile currentTile = new CTile(2, 6);
                CTile proposedTile = new CTile(2, 3);
                Assert.IsFalse(IsMovementDiagonal(currentTile, proposedTile));
                Assert.IsTrue(IsMovementStraight(currentTile, proposedTile));
                Assert.IsTrue(IsSpaceBetweenTilesFree(occupancy, currentTile, proposedTile));
            }
        }

        #endregion
    }
}
