using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class ProfileControl : UserControl
    {
        internal ProfileControl(CMainPageViewModel mainPageViewModel, CProfileModel model)
        {
            InitializeComponent();
            DataContext = new CProfileControlViewModel(mainPageViewModel, model);
            foreach (CGameInfoModel gameInfoModel in model.GamesInfo)
                GamesList.Children.Add(new GameInfoItem(mainPageViewModel, gameInfoModel));
        }
    }
}
