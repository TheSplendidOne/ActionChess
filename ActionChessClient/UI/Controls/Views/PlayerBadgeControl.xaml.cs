using System;
using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class PlayerBadgeControl : UserControl
    {
        public PlayerBadgeControl(String nickname, Int32 winRate)
        {
            InitializeComponent();
            DataContext = new CPlayerBadgeControlViewModel(nickname, winRate);
        }
    }
}
