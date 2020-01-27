using System;
using System.Threading;
using System.Threading.Tasks;

namespace Animator
{
    public class CPointAnimator<T> : IDisposable
    where T : IPointAnimatable
    {
        public T Animatable { get; }

        public CPoint From { get; private set; }

        public CPoint To { get; private set; }

        public TimeSpan TotalDuration { get; private set; }

        public EAnimatorState State { get; private set; }

        private readonly Object _locker = new Object();

        public event Action AnimationCompleted;

        private DateTime _startTime;

        private CVector _vector;

        private Task _currentTask;

        private CancellationTokenSource _cancellationTokenSource;

        private Boolean _isDisposed;

        public CPointAnimator(T animatable, CPoint from, CPoint to, TimeSpan totalDuration)
        {
            if (totalDuration <= TimeSpan.Zero)
                throw new ArgumentException("Total duration value should be more than zero");
            Animatable = animatable;
            From = from;
            To = to;
            TotalDuration = totalDuration;
            State = EAnimatorState.Unstarted;
        }

        public void BeginAnimation()
        {
            lock (_locker)
            {
                if (State != EAnimatorState.Unstarted && State != EAnimatorState.Suspended)
                    throw new InvalidOperationException($"Cannot begin animation because it {State}");
                _startTime = DateTime.UtcNow;
                _vector = new CVector(From, To);
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = _cancellationTokenSource.Token;
                _currentTask = new Task(() => DoAnimation(token), token);
                _currentTask.Start();
                State = EAnimatorState.Running;
            }
        }

        public void CancelAnimation()
        {
            lock (_locker)
            {
                if (State != EAnimatorState.Running)
                    throw new InvalidOperationException($"Cannot cancel animation because it {State}");
                State = EAnimatorState.Suspended;
                _cancellationTokenSource.Cancel();
            }
        }

        public void ProlongAnimation(CPoint from, CPoint to, TimeSpan totalDuration)
        {
            lock (_locker)
            {
                if (State != EAnimatorState.Running)
                    throw new InvalidOperationException($"Cannot prolong animation because it {State}");
                CancelAnimation();
                From = from;
                To = to;
                TotalDuration = totalDuration;
                BeginAnimation();
            }
        }

        private void DoAnimation(CancellationToken token)
        {
            TimeSpan durationFromStart;
            while ((durationFromStart = (DateTime.UtcNow - _startTime).Duration()) < TotalDuration)
            {
                if (token.IsCancellationRequested) return;
                Double completedPart = durationFromStart.TotalMilliseconds / TotalDuration.TotalMilliseconds;
                Animatable.SetAnimationValue(From.MovePoint(completedPart * _vector));
                token.WaitHandle.WaitOne(5);
            }
            Animatable.SetAnimationValue(To);
            if (!token.IsCancellationRequested)
                new Task(() => AnimationCompleted?.Invoke()).Start();
            State = EAnimatorState.Stopped;
        }

        public void Dispose()
        {
            lock (_locker)
            {
                if (_isDisposed)
                    return;
                _cancellationTokenSource.Dispose();
                _isDisposed = true;
            }
        }
    }
}