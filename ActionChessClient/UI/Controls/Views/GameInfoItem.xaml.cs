using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class GameInfoItem : UserControl
    {
        public GameInfoItem(CMainPageViewModel mainPageViewModel, CGameInfoModel gameInfo)
        {
            InitializeComponent();
            DataContext = new CGameInfoItemControlViewModel(mainPageViewModel, gameInfo);
        }
    }
}
