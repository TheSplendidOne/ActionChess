using System;
using System.Windows;
using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class LogInControl : UserControl
    {
        internal LogInControl(CMainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            ((CLogInControlViewModel) DataContext).ParentViewModel = mainPageViewModel;
        }

        private void Password_OnPasswordChanged(Object sender, RoutedEventArgs e)
        {
            ((CLogInControlViewModel) DataContext).Password = ((PasswordBox)sender).Password;
        }
    }
}
