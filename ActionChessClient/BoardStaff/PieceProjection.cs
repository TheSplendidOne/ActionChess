using System;
using System.Windows.Controls;

namespace ActionChessClient
{
    internal class CPieceProjection : Image, IDisposable
    {
        public static readonly Double ValidStateOpacity = 0.75;

        public static readonly Double InvalidStateOpacity = 0;

        private Boolean _isValidState;

        private Boolean _isDisposed;

        public CPiece Root { get; }

        public Boolean IsValidState
        {
            get => _isValidState;
            set
            {
                Opacity = value ? ValidStateOpacity : InvalidStateOpacity;
                _isValidState = value;
            }
        }

        public CPieceProjection(CPiece root)
        {
            Root = root;
            Source = root.Source;
            AllowDrop = root.AllowDrop;
            Width = root.Width;
            Height = root.Height;
            Margin = root.Margin;
            Opacity = ValidStateOpacity;
        }

        public void Dispose()
        {
            if(!_isDisposed)
                ((Panel)Parent).Children.Remove(this);
        }
    }
}