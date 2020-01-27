using System;
using System.Text.RegularExpressions;

namespace ActionChessClient
{
    internal class CSignUpControlViewModel : CMainPageControlViewModel
    {
        private static readonly Regex ValidNicknameRegex = new Regex(@"^\w{3,24}$");

        private static readonly Regex ValidPasswordRegex = new Regex(@"^\w{8,24}$");

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

        private String _repeatPassword;

        public String RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
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

        private CInstantCommand _signUpCommand;

        public CInstantCommand SignUpCommand
        {
            get { return _signUpCommand ??= new CInstantCommand(TrySignUp); }
        }

        private CInstantCommand _toLogInCommand;

        public CInstantCommand ToLogInCommand
        {
            get { return _toLogInCommand ??= new CInstantCommand(ToLogIn); }
        }

        public Boolean IsNicknameValid => ValidNicknameRegex.IsMatch(_nickname);

        public Boolean IsPasswordValid => ValidPasswordRegex.IsMatch(_password);

        private void TrySignUp(Object data)
        {
            if (String.IsNullOrEmpty(Nickname))
            {
                WarningMessage = "Enter your nickname";
                return;
            }

            if (!IsNicknameValid)
            {
                WarningMessage = "Invalid nickname";
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                WarningMessage = "Enter your password";
                return;
            }

            if (!IsPasswordValid)
            {
                WarningMessage = "Invalid password";
                return;
            }

            if (String.IsNullOrEmpty(RepeatPassword))
            {
                WarningMessage = "Repeat your password";
                return;
            }

            if (Password != RepeatPassword)
            {
                WarningMessage = "Passwords must match";
                return;
            }

            if (CAuthenticationStaff.Instance.Client.TrySignUp(_nickname, _password))
            {
                ParentViewModel.ActualControl = new LogInControl(ParentViewModel);
            }
        }

        private void ToLogIn(Object data)
        {
            ParentViewModel.ActualControl = new LogInControl(ParentViewModel);
        }
    }
}
