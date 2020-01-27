using System;

namespace ActionChessClient
{
    internal class CLogInControlViewModel : CMainPageControlViewModel
    {

        private String _nickname;

        public String Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                OnPropertyChanged();
            }
        }

        private String _password;

        public String Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private String _warningMessage;

        public String WarningMessage
        {
            get => _warningMessage;
            set
            {
                _warningMessage = value;
                OnPropertyChanged();
            }
        }

        private CInstantCommand _logInCommand;
        public CInstantCommand LogInCommand
        {
            get { return _logInCommand ??= new CInstantCommand(TryLogIn); }
        }

        private CInstantCommand _toSignUpCommand;
        public CInstantCommand ToSignUpCommand
        {
            get { return _toSignUpCommand ??= new CInstantCommand(ToSignUp); }
        }

        private CInstantCommand _toRememberPasswordCommand;
        public CInstantCommand ToRememberPasswordCommand
        {
            get { return _toRememberPasswordCommand ??= new CInstantCommand(ToRememberPassword); }
        }

        private void TryLogIn(Object data)
        {
            if (String.IsNullOrEmpty(Nickname))
            {
                WarningMessage = "Enter your nickname";
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                WarningMessage = "Enter your password";
                return;
            }

            if ((CAuthenticationStaff.Instance.User = CAuthenticationStaff.Instance.Client.TryLogIn(_nickname, _password)) != null)
            {
                ParentViewModel.ActualControl = new MenuControl(ParentViewModel);
            }
        }

        private void ToSignUp(Object data)
        {
            ParentViewModel.ActualControl = new SignUpControl(ParentViewModel);
        }

        private void ToRememberPassword(Object data)
        {

        }
    }
}
