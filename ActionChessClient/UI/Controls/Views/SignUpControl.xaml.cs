using System;
using System.Windows;
using System.Windows.Controls;

namespace ActionChessClient
{
    internal partial class SignUpControl : UserControl
    {
        internal SignUpControl(CMainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            ((CSignUpControlViewModel) DataContext).ParentViewModel = mainPageViewModel;
        }

        private void Password_OnPasswordChanged(Object sender, RoutedEventArgs e)
        {
            ((CSignUpControlViewModel) DataContext).Password = ((PasswordBox)sender).Password;
        }

        private void RepeatPassword_OnPasswordConfirmationChanged(Object sender, RoutedEventArgs e)
        {
            ((CSignUpControlViewModel)DataContext).RepeatPassword = ((PasswordBox)sender).Password;
        }
    }
}
