using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ActionChessClient.Extensions;
using ActionChessClient.GameService;

namespace ActionChessClient
{
    internal class CBoardControlPresenter : CBoardControlPresenterBase, IVirtualBoardServiceCallback
    {
        private readonly VirtualBoardServiceClient _virtualBoardService;

        private readonly Guid _gameId;

        private readonly BoardControl _attachedBoard;
        
        private readonly CBoardOccupancy _boardStates;

        private readonly Dictionary<Int32, CPiece> _pieces;

        private readonly HashSet<CTile> _attachedTiles;

        public CBoardControlPresenter(Guid gameId, EPieceColor mySideColor, BoardControl attachedBoard)
        {
            _gameId = gameId;
            _attachedBoard = attachedBoard;
            _virtualBoardService = new VirtualBoardServiceClient(new InstanceContext(this));
            _boardStates = new CBoardOccupancy();
            _pieces = new Dictionary<Int32, CPiece>();
            _attachedTiles = new HashSet<CTile>();
            InitializeBoardView(mySideColor);
            _virtualBoardService.ConnectBoard(_gameId, CAuthenticationStaff.Instance.User.UserId);
        }

        #region InitializeBoardViewStaff

        private void InitializeBoardView(EPieceColor mySideColor)
        {
            InitializePiecesCollection();
            AttachEventHandlersToPieces(mySideColor);
            ShowPieces();
            AttachEventHandlersToBoard();
        }

        protected override void InitializePiecesCollection()
        {
            void InitializeSide(Int32 yPieceCoordinate, Int32 firstPieceId, EPieceColor sideColor, Boolean mirrorOverX)
            {
                Int32 xPieceCoordinate;
                for (xPieceCoordinate = 0; xPieceCoordinate < 8; xPieceCoordinate++)
                    _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Pawn, sideColor, new CTile(xPieceCoordinate, yPieceCoordinate)));

                if (mirrorOverX)
                    --yPieceCoordinate;
                else
                    ++yPieceCoordinate;

                xPieceCoordinate = 0;

                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Rook, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Knight, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Bishop, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Queen, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.King, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Bishop, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId++, EPieceType.Knight, sideColor, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CPiece.Create(firstPieceId, EPieceType.Rook, sideColor, new CTile(xPieceCoordinate, yPieceCoordinate)));
            }

            InitializeSide(6, 1, EPieceColor.White, false);
            InitializeSide(1, 17, EPieceColor.Black, true);
        }

        private void AttachEventHandlersToPieces(EPieceColor mySideColor)
        {
            foreach (CPiece piece in _pieces.Values)
            {
                if(piece.Color == mySideColor)
                    piece.MouseDown += MouseDownPieceEventHandler;
                piece.MoveAnimationCompletedEventHandler = GetMovementAnimationCompletedEventHandler(_boardStates, piece);
            }
        }

        protected override void ShowPieces()
        {
            foreach (CPiece piece in _pieces.Values)
            {
                Panel.SetZIndex(piece, (Int32)EZIndex.Piece);
                _attachedBoard.BoardCanvas.Children.Add(piece);
            }
        }

        private void AttachEventHandlersToBoard()
        {
            _attachedBoard.BoardCanvas.DragOver += DragOverProjectionEventHandler;
            _attachedBoard.BoardCanvas.Drop += DropProjectionEventHandler;
        }

        #endregion

        #region DragDropStaff

        private void MouseDownPieceEventHandler(Object pieceObject, MouseEventArgs args)
        {
            CPiece piece = (CPiece)pieceObject;
            if (piece.IsActive)
            {
                using CPieceProjection projection = PutNewProjectionOnBoard(piece);
                DragDrop.DoDragDrop(projection.Parent, projection, DragDropEffects.Move);
            }
        }

        private CPieceProjection PutNewProjectionOnBoard(CPiece piece)
        {
            CPieceProjection projection = new CPieceProjection(piece);
            _attachedBoard.BoardCanvas.Children.Add(projection);
            Panel.SetZIndex(projection, (Int32)EZIndex.PieceProjection);
            return projection;
        }

        private void DragOverProjectionEventHandler(Object board, DragEventArgs projectionArgs)
        {
            CPieceProjection projection = projectionArgs.GetSingle<CPieceProjection>();
            Point cursorCoordinates = projectionArgs.GetPosition((IInputElement)board);
            ChangeProjectionState(projection, cursorCoordinates);
        }

        private void ChangeProjectionState(CPieceProjection projection, Point cursorCoordinates)
        {
            CTile cursorTile = new CTile(cursorCoordinates);
            projection.Margin = new Thickness(cursorTile.X * CPiece.PieceSize, cursorTile.Y * CPiece.PieceSize, 0, 0);
            projection.IsValidState = IsCursorOverBoard(cursorCoordinates) &&
                                      !_attachedTiles.Contains(cursorTile) &&
                                      projection.Root.IsMoveValid(_boardStates, cursorTile);
        }

        private static Boolean IsCursorOverBoard(Point cursorCoordinates)
        {
            return cursorCoordinates.X >= 0 &&
                   cursorCoordinates.X < (Double)Application.Current.Resources["BoardSize"] &&
                   cursorCoordinates.Y >= 0 &&
                   cursorCoordinates.Y < (Double)Application.Current.Resources["BoardSize"];
        }

        private void DropProjectionEventHandler(Object boardObject, DragEventArgs projectionArgs)
        {
            CPieceProjection projection = projectionArgs.GetSingle<CPieceProjection>();
            if (projection.IsValidState)
            {
                projection.Root.AttachedTile = new CTile(projection.Margin);
                _attachedTiles.Add(projection.Root.AttachedTile);
                _virtualBoardService.TryMovePiece(_gameId, projection.Root.Id, new CPoint { X = projection.Margin.Left, Y = projection.Margin.Top });
            }
        }

        #endregion

        #region MovementAnimationStaff

        private EventHandler GetMovementAnimationCompletedEventHandler(CBoardOccupancy states, CPiece piece)
        {
            return (obj, args) =>
            {
                states[new CTile(piece.Margin)] = piece.Color;
                RemoveAttachedTile(piece);
                Rectangle timeoutIndicator = PutNewTimeoutIndicatorOnBoard(piece.Margin);
                piece.AttachedTimeoutIndicator = timeoutIndicator;
                DoubleAnimation timeoutIndicatorAnimation = GetNewTimeoutIndicatorAnimation();
                timeoutIndicatorAnimation.Completed += GetTimeoutIndicatorAnimationCompletedEventHandler(piece);
                timeoutIndicator.BeginAnimation(Rectangle.HeightProperty, timeoutIndicatorAnimation);
            };
        }

        private void RemoveAttachedTile(CPiece piece)
        {
            if (piece.AttachedTile != null)
            {
                _attachedTiles.Remove(piece.AttachedTile);
                piece.AttachedTile = null;
            }
        }

        private Rectangle PutNewTimeoutIndicatorOnBoard(Thickness position)
        {
            Rectangle indicator = new Rectangle
            {
                Width = CPiece.PieceSize,
                Height = CPiece.PieceSize,
                Fill = (SolidColorBrush)Application.Current.Resources["TimeoutRectangleBrush"],
                Opacity = 0.75,
                Margin = new Thickness(position.Left, 0, 0, 0)
            };
            _attachedBoard.BoardCanvas.Children.Add(indicator);
            Panel.SetZIndex(indicator, (Int32)EZIndex.TimeoutRectangle);
            Canvas.SetBottom(indicator, (Double)Application.Current.Resources["BoardSize"] - position.Top - CPiece.PieceSize);
            return indicator;
        }

        private static DoubleAnimation GetNewTimeoutIndicatorAnimation()
        {
            return new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(CPiece.Timeout)
            };
        }

        private EventHandler GetTimeoutIndicatorAnimationCompletedEventHandler(CPiece piece)
        {
            return (obj, args) =>
            {
                piece.IsActive = true;
                _attachedBoard.BoardCanvas.Children.Remove(piece.AttachedTimeoutIndicator);
                piece.AttachedTimeoutIndicator = null;
            };
        }

        #endregion

        #region ServiceCallbacks

        public void MovePiece(Int32 pieceId, CPoint newPosition)
        {
            CPiece piece = _pieces[pieceId];
            _boardStates.ResetToDefault(new CTile(piece.Margin));
            piece.IsActive = false;
            piece.AnimatePiece(new Thickness(newPosition.X, newPosition.Y, 0, 0));
        }

        public void HandleCollision(Int32 invaderId, Int32 capturedId, Boolean isCollisionStopsMoving)
        {
            if (!_pieces.ContainsKey(invaderId))
                throw new InvalidOperationException($"Invader piece {invaderId} already captured");
            if (!_pieces.ContainsKey(capturedId))
                throw new InvalidOperationException($"Captured piece {capturedId} already captured");
            CPiece invaderPiece = _pieces[invaderId];
            CPiece capturedPiece = _pieces[capturedId];
            _attachedBoard.BoardCanvas.Children.Remove(capturedPiece);
            if (isCollisionStopsMoving)
            {
                invaderPiece.StopAnimation();
                invaderPiece.AnimatePiece(capturedPiece.Margin);
            }
            else if (capturedPiece.CurrentAnimation != null)
            {
                capturedPiece.StopAnimation();
                if(capturedPiece.AttachedTile != null)
                    _attachedTiles.Remove(capturedPiece.AttachedTile);
            }
            if(capturedPiece.AttachedTimeoutIndicator != null)
                _attachedBoard.BoardCanvas.Children.Remove(capturedPiece.AttachedTimeoutIndicator);
            _pieces.Remove(capturedId);
        }

        public void TransformPawnToQueen(Int32 pieceId)
        {
            _pieces[pieceId].TransformToQueen();
        }

        #endregion
    }
}
