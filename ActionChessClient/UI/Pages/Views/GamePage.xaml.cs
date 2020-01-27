using System.ComponentModel;
using System.Windows.Controls;
using DataBaseAccessService;
using SharedServiceLibrary;

namespace ActionChessClient
{
    internal partial class GamePage : Page
    {
        public GamePage(CGameStartModel model)
        {
            InitializeComponent();
            IDataBaseAccessService client = SDbService.Get();
            switch (model.MySideColor)
            {
                case EPieceColor.White:
                    WhitePlayerBadge.Content = new PlayerBadgeControl(
                        CAuthenticationStaff.Instance.User.Nickname,
                        client.GetWinRate(CAuthenticationStaff.Instance.User.UserId));
                    BlackPlayerBadge.Content = new PlayerBadgeControl(
                        client.FindUserById(model.OpponentId).Nickname,
                        client.GetWinRate(model.OpponentId));
                    break;
                case EPieceColor.Black:
                    WhitePlayerBadge.Content = new PlayerBadgeControl(
                        client.FindUserById(model.OpponentId).Nickname,
                        client.GetWinRate(model.OpponentId));
                    BlackPlayerBadge.Content = new PlayerBadgeControl(
                        CAuthenticationStaff.Instance.User.Nickname,
                        client.GetWinRate(CAuthenticationStaff.Instance.User.UserId));
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown or undefined player side color");
            }
            DataContext = new CGamePageViewModel(model.GameId, model.MySideColor);
        }
    }
}
