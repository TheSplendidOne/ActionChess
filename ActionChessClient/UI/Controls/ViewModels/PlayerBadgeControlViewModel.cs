using System;

namespace ActionChessClient
{
    internal class CPlayerBadgeControlViewModel : CViewModelBase
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

        private String _winRate;

        public String WinRate
        {
            get => _winRate;
            set
            {
                _winRate = value;
                OnPropertyChanged();
            }
        }

        public CPlayerBadgeControlViewModel(String nickname, Int32 winRate)
        {
            Nickname = nickname;
            WinRate = "WinRate: " + winRate;
        }
    }
}
