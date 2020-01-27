using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class MenuControl : UserControl
    {
        internal MenuControl(CMainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            ((CMenuControlViewModel) DataContext).ParentViewModel = mainPageViewModel;
        }
    }
}
