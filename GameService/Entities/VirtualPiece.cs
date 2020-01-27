using System;
using Animator;

namespace GameService
{
    internal class CVirtualPiece : IPointAnimatable
    {
        public static readonly Double PieceSize = 86;

        public CPointAnimator<CVirtualPiece> Animation { get; private set; }

        public DateTime? AnimationStartTime { get; private set; }

        public CPoint Position { get; private set; }

        public CVirtualPiece(CPoint startPosition)
        {
            Position = startPosition;
        }

        public CVirtualPiece(Int32 xBoardCell, Int32 yBoardCell)
        {
            Position = new CPoint(xBoardCell * PieceSize, yBoardCell * PieceSize);
        }

        public void AnimateTo(CPoint to, TimeSpan runtime, Action animationCompletedAction)
        {
            if(Animation != null)
                throw new InvalidOperationException("Piece animate now");
            Animation = new CPointAnimator<CVirtualPiece>(this, Position, to, runtime);
            Animation.AnimationCompleted += ResetAnimationToDefault;
            Animation.AnimationCompleted += animationCompletedAction;
            AnimationStartTime = DateTime.UtcNow;
            Animation.BeginAnimation();
        }

        public void ResetAnimationToDefault()
        {
            if(Animation == null)
                throw new InvalidOperationException(@"Piece didn't animate now");
            if(Animation.State == EAnimatorState.Running)
                Animation.CancelAnimation();
            Animation.Dispose();
            Animation = null;
            AnimationStartTime = null;
        }

        public void ProlongAnimation(CPoint newPosition, TimeSpan runtime)
        {
            if (Animation.State != EAnimatorState.Running)
                throw new InvalidOperationException(@"Piece didn't animate now");
            Animation.ProlongAnimation(Position, newPosition, runtime);
        }

        public void SetAnimationValue(CPoint newValue)
        {
            Position = newValue;
        }
    }
}
