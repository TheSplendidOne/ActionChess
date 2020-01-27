using System;
using Animator;

namespace GameService
{
    internal class CBoardSynchronizer : IDisposable
    {
        public CPlayerSide White { get; }

        public CPlayerSide Black { get; }

        private readonly CBoardChecker _checker;

        private readonly CBoardRecorder _recorder;

        private readonly Object _disposeLocker = new Object();

        private Boolean _isDisposed;

        public CBoardSynchronizer(CPlayer whitePlayer, CPlayer blackPlayer, CBoardRecorder recorder, Action<ESideColor> endRoundAction)
        {
            White = new CPlayerSide(whitePlayer, SPiecesKitFactory.GetPiecesStartKit(ESideColor.White), SPiecesKitFactory.GetPawns(ESideColor.White));
            Black = new CPlayerSide(blackPlayer, SPiecesKitFactory.GetPiecesStartKit(ESideColor.Black), SPiecesKitFactory.GetPawns(ESideColor.Black));
            _recorder = recorder;
            _checker = new CBoardChecker(White, Black, recorder, endRoundAction);
        }

        public void MovePiece(Int32 pieceId, CPoint newPosition)
        {
            CPlayerSide side;
            if (White.AllPieces.ContainsKey(pieceId))
            {
                side = White;
            }
            else
            {
                if (Black.AllPieces.ContainsKey(pieceId))
                    side = Black;
                else
                    return;
            }
            if (side.MovingPieces.ContainsKey(pieceId))
                return;
            CVirtualPiece piece = side.AllPieces[pieceId];
            side.MovingPieces.Add(pieceId, piece);
            _recorder.AddEntry(new CMovementBeganEntry(pieceId, piece.Position));
            piece.AnimateTo(newPosition, CalculateRuntime(piece.Position, newPosition), () =>
            {
                side.DelayRemoveStoppedPiecesKeys.Add(pieceId);
                _recorder.AddMovementFinishedEntry(new CMovementFinishedEntry(pieceId, newPosition));
            });
            White.Player.VirtualBoardContext.GetCallbackChannel<IVirtualBoardCallback>().MovePiece(pieceId, newPosition);
            Black.Player.VirtualBoardContext.GetCallbackChannel<IVirtualBoardCallback>().MovePiece(pieceId, newPosition);
        }

        public static TimeSpan CalculateRuntime(CPoint first, CPoint second)
        {
            return TimeSpan.FromSeconds(new CVector(first, second).Length / CVirtualPiece.PieceSize);
        }

        public CVirtualPiece GetPieceById(Int32 pieceId)
        {
            if (White.AllPieces.ContainsKey(pieceId))
                return White.AllPieces[pieceId];
            if (Black.AllPieces.ContainsKey(pieceId))
                return Black.AllPieces[pieceId];
            return null;
        }

        public void Dispose()
        {
            lock (_disposeLocker)
            {
                if (_isDisposed)
                    return;
                _checker.Dispose();
                _isDisposed = true;
            }
        }
    }
}