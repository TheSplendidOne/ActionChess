using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Animator;

namespace GameService
{
    internal class CBoardChecker : IDisposable
    {
        private static readonly Double CollisionBound = CVirtualPiece.PieceSize * 0.3;

        private static readonly Int32 CheckInterval = 15;

        private readonly Object _checkerLocker = new Object();

        private readonly Object _disposeLocker = new Object();

        private readonly Timer _timer;

        private readonly CBoardRecorder _recorder;

        private readonly Action<ESideColor> _endRoundAction;

        private Boolean _isDisposed;

        public CPlayerSide White { get; }

        public CPlayerSide Black { get; }

        public CBoardChecker(CPlayerSide white, CPlayerSide black, CBoardRecorder recorder, Action<ESideColor> endRoundAction)
        {
            White = white;
            Black = black;
            _recorder = recorder;
            _endRoundAction = endRoundAction;
            _timer = new Timer(CheckBoard, null, CheckInterval, CheckInterval);
        }

        private Double GetPawnToQueenTransformationBorder(ESideColor color)
        {
            return color switch
            {
                ESideColor.White => 0, // Pawn to queen transformation border for white side 
                ESideColor.Black => CVirtualPiece.PieceSize * 7, // Pawn to queen transformation border for black side 
                _ => throw new InvalidEnumArgumentException($"Unknown kit color: {color}")
            };
        }

        private void CheckBoard(Object obj)
        {
            lock (_checkerLocker)
            {
                if (_isDisposed) return;
                CheckAllCollisions();
                RemoveAllStoppedPieces();
                CheckPawns();
                CheckSurvivingKings();
            }
        }

        private void RemoveAllStoppedPieces()
        {
            White.RemoveStoppedPieces();
            Black.RemoveStoppedPieces();
        }

        private void CheckSurvivingKings()
        {
            ESideColor? winner = null;
            if (!White.AllPieces.ContainsKey(SPiecesKitFactory.WhiteKingId))
            {
                winner = ESideColor.Black;
            }
            if (!Black.AllPieces.ContainsKey(SPiecesKitFactory.BlackKingId))
            {
                winner = ESideColor.White;
            }
            if (winner != null)
            {
                Dispose();
                _endRoundAction.Invoke((ESideColor)winner);
            }
        }

        private void CheckPawns()
        {
            CheckPawnsFromSingleSide(White);
            CheckPawnsFromSingleSide(Black);
        }

        private void CheckPawnsFromSingleSide(CPlayerSide side)
        {
            Double transformationBorder = GetPawnToQueenTransformationBorder((ESideColor) side.Player.SideColor);
            List<Int32> delayRemovePawns = new List<Int32>();
            foreach (Int32 pawnKey in side.PawnsKeys)
            {
                if (side.AllPieces[pawnKey].Position.Y == transformationBorder)
                {
                    White.Player.VirtualBoardContext.GetCallbackChannel<IVirtualBoardCallback>().TransformPawnToQueen(pawnKey);
                    Black.Player.VirtualBoardContext.GetCallbackChannel<IVirtualBoardCallback>().TransformPawnToQueen(pawnKey);
                    _recorder.AddEntry(new CPawnTransformedEntry(pawnKey, side.Player.SideColor == ESideColor.White));
                    delayRemovePawns.Add(pawnKey);
                }
            }
            foreach (Int32 pawn in delayRemovePawns)
            {
                side.PawnsKeys.Remove(pawn);
            }
        }

        private void CheckAllCollisions()
        {
            CheckCollisionsFromSingleSide(White, Black);
            Black.RemoveCapturedPieces();
            CheckCollisionsFromSingleSide(Black, White);
            White.RemoveCapturedPieces();
        }

        private void CheckCollisionsFromSingleSide(CPlayerSide invader, CPlayerSide defender)
        {
            foreach (Int32 invaderPieceKey in invader.MovingPieces.Keys)
            {
                CVirtualPiece invaderPiece = invader.MovingPieces[invaderPieceKey];
                foreach (Int32 defenderPieceKey in defender.AllPieces.Keys)
                {
                    CVirtualPiece defenderPiece = defender.AllPieces[defenderPieceKey];
                    if (CheckCollision(invaderPiece, defenderPiece))
                    {
                        if (defenderPiece.AnimationStartTime == null)
                            ProcessCollision(invader, defender, invaderPieceKey, defenderPieceKey, true);
                        else
                        {
                            if (invaderPiece.AnimationStartTime < defenderPiece.AnimationStartTime)
                                ProcessCollision(invader, defender, invaderPieceKey, defenderPieceKey, false);
                        }
                    }
                }
            }
        }

        private static Boolean CheckCollision(CVirtualPiece first, CVirtualPiece second)
        {
            return new CVector(first.Position, second.Position).Length < CollisionBound;
        }

        private void ProcessCollision(CPlayerSide invader, CPlayerSide defender, Int32 invaderPieceId, Int32 capturedPieceId, Boolean isCollisionStopsMoving)
        {
            CVirtualPiece invaderPiece = invader.MovingPieces[invaderPieceId];
            CVirtualPiece capturedPiece = defender.AllPieces[capturedPieceId];
            if (isCollisionStopsMoving)
            {
                invaderPiece.ProlongAnimation(capturedPiece.Position,
                    CBoardSynchronizer.CalculateRuntime(invaderPiece.Position, capturedPiece.Position));
            }
            else
            {
                capturedPiece.ResetAnimationToDefault();
                _recorder.AddMovementFinishedEntry(new CMovementFinishedEntry(capturedPieceId, capturedPiece.Position));
            }
            _recorder.AddEntry(new CPieceRemovedEntry(capturedPieceId));
            defender.DelayRemovePiecesKeys.Add(capturedPieceId);
            White.Player.VirtualBoardContext.GetCallbackChannel<IVirtualBoardCallback>().
                HandleCollision(invaderPieceId, capturedPieceId, isCollisionStopsMoving);
            Black.Player.VirtualBoardContext.GetCallbackChannel<IVirtualBoardCallback>().
                HandleCollision(invaderPieceId, capturedPieceId, isCollisionStopsMoving);
        }

        public void Dispose()
        {
            lock (_disposeLocker)
            {
                if (_isDisposed)
                    return;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _isDisposed = true;
            }
        }
    }
}
