using System;
using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class BoardControl : UserControl
    {
        public BoardControl(Guid gameId, EPieceColor mySideColor)
        {
            InitializeComponent();
            DataContext = new CBoardControlPresenter(gameId, mySideColor, this);
        }
    }
}
