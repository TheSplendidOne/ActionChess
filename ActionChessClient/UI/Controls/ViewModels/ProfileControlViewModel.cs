using System;

namespace ActionChessClient
{
    internal class CProfileControlViewModel : CMainPageControlViewModel
    {
        public CProfileControlViewModel(CMainPageViewModel mainPageViewModel, CProfileModel model)
        {
            ParentViewModel = mainPageViewModel;
            Nickname = model.Nickname;
            RegistrationDate = model.RegistrationDate.ToShortDateString();
            GamesCount = model.GamesCount;
            WinRate = model.WinRate;
        }

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

        private String _registrationDate;

        public String RegistrationDate
        {
            get => _registrationDate;
            set
            {
                _registrationDate = value;
                OnPropertyChanged();
            }
        }

        private Int32 _gamesCount;

        public Int32 GamesCount
        {
            get => _gamesCount;
            set
            {
                _gamesCount = value;
                OnPropertyChanged();
            }
        }

        private Int32 _winRate;

        public Int32 WinRate
        {
            get => _winRate;
            set
            {
                _winRate = value;
                OnPropertyChanged();
            }
        }

        private CInstantCommand _toMenuCommand;

        public CInstantCommand ToMenuCommand
        {
            get { return _toMenuCommand ??= new CInstantCommand(ToMenu); }
        }

        private void ToMenu(Object data)
        {
            ParentViewModel.ActualControl = new MenuControl(ParentViewModel);
        }
    }
}
