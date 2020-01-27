using System;
using System.IO;

namespace ActionChessClient
{
    internal class CNavigator
    {
        private static readonly Lazy<CNavigator> _instance = new Lazy<CNavigator>(() => new CNavigator());

        public static CNavigator Instance => _instance.Value;

        public event Action MainPageNavigate;

        public event Action<CGameStartModel> GamePageNavigate;

        public void NavigateToMainPage()
        {
            MainPageNavigate?.Invoke();
        }

        public void NavigateToGamePage(CGameStartModel model)
        {
            GamePageNavigate?.Invoke(model);
        }

    }
}
