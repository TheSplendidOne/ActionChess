using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ActionChessClient
{
    internal class CPiece : Image
    {
        public static readonly Int32 PieceSize = 86;

        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(12);

        private CMovementValidator _movementValidator;

        public Int32 Id { get; }

        public Boolean IsActive { get; set; }

        public EPieceColor Color { get; }

        public CTile AttachedTile { get; set; }

        public Rectangle AttachedTimeoutIndicator { get; set; }

        public ThicknessAnimation CurrentAnimation { get; set; }

        public EventHandler MoveAnimationCompletedEventHandler { get; set; }

        public static CPiece Create(Int32 id, EPieceType pieceType, EPieceColor pieceColor, CTile tile)
        {
            return new CPiece(
                id,
                new BitmapImage(new Uri($"/Images/{pieceColor}/{pieceType}.png", UriKind.RelativeOrAbsolute)),
                SMovementValidatorFactory.GetMovementValidator(pieceType),
                pieceColor,
                tile);
        }

        private CPiece(Int32 id, ImageSource source, CMovementValidator movementValidator, EPieceColor pieceColor, CTile tile)
        {
            Source = source;
            _movementValidator = movementValidator;
            Color = pieceColor;
            IsActive = true;
            Id = id;
            Width = PieceSize;
            Height = PieceSize;
            Margin = new Thickness(tile.X * PieceSize, tile.Y * PieceSize, 0, 0);
            AllowDrop = true;
        }

        public Boolean IsMoveValid(CBoardOccupancy boardStates, CTile proposedTile)
        {
            return _movementValidator.CheckAll(boardStates, new CTile(Margin), proposedTile);
        }

        public void AnimatePiece(Thickness newMargin)
        {
            var animation = new ThicknessAnimation
            {
                To = newMargin,
                SpeedRatio = CPiece.PieceSize / Margin.Difference(newMargin)
            };
            animation.Completed += MoveAnimationCompletedEventHandler;
            CurrentAnimation = animation;
            BeginAnimation(MarginProperty, animation);
        }

        public void StopAnimation()
        {
            CurrentAnimation.BeginTime = null;
            CurrentAnimation.Completed -= MoveAnimationCompletedEventHandler;
            BeginAnimation(MarginProperty, CurrentAnimation);
            CurrentAnimation = null;
        }

        public void TransformToQueen()
        {
            _movementValidator = SMovementValidatorFactory.GetMovementValidator(EPieceType.Queen);
            Source = new BitmapImage(new Uri($"/Images/{Color}/Queen.png", UriKind.RelativeOrAbsolute));
        }
    }
}