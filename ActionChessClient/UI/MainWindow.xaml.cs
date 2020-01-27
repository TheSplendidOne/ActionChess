using System;
using System.Windows;
using System.Windows.Input;

namespace ActionChessClient
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CNavigator.Instance.MainPageNavigate += ToMainPage;
            CNavigator.Instance.GamePageNavigate += ToGamePage;
            MainFrame.Content = new MainPage(EMainPageControl.LogIn);
        }

        private void ToMainPage()
        {
            MainFrame.Content = new MainPage(EMainPageControl.Menu);
        }

        private void ToGamePage(CGameStartModel model)
        {
            MainFrame.Content = new GamePage(model);
        }

        private void MainWindow_OnClosed(Object sender, EventArgs e)
        {
            if (CAuthenticationStaff.Instance.User != null)
                CAuthenticationStaff.Instance.Client.LogOutAsync(CAuthenticationStaff.Instance.User.UserId);
            if(MainFrame.Content is GamePage gamePage)
                WindowInvokeLeaveGame(gamePage);
        }

        private void MainWindow_OnKeyUp(Object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape && MainFrame.Content is GamePage gamePage)
                WindowInvokeLeaveGame(gamePage);
        }

        private static void WindowInvokeLeaveGame(GamePage gamePage)
        {
            ((CGamePageViewModel)gamePage.DataContext).LeaveGameCommand.Execute(null);
        }
    }
}
