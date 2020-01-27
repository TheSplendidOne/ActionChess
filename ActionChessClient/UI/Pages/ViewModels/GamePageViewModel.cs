using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ActionChessClient.GameService;

namespace ActionChessClient
{
    internal class CGamePageViewModel : CViewModelBase, IGameManagerServiceCallback
    {
        private static readonly Int32 MaxScore = 2;

        private readonly Guid _gameId;

        private readonly EPieceColor _mySideColor;

        private readonly GameManagerServiceClient _gameManagerService;

        public CGamePageViewModel(Guid gameId, EPieceColor mySideColor)
        {
            _gameId = gameId;
            _mySideColor = mySideColor;
            _gameManagerService = new GameManagerServiceClient(new InstanceContext(this));
            _gameManagerService.ConnectGameManager(gameId, CAuthenticationStaff.Instance.User.UserId);
            GameBoardContent = new BoardControl(gameId, mySideColor);
        }

        private Int32 _whitePlayerScore;

        public Int32 WhitePlayerScore
        {
            get => _whitePlayerScore;
            set
            {
                _whitePlayerScore = value;
                OnPropertyChanged();
            }
        }

        private Int32 _blackPlayerScore;

        public Int32 BlackPlayerScore
        {
            get => _blackPlayerScore;
            set
            {
                _blackPlayerScore = value;
                OnPropertyChanged();
            }
        }

        private BoardControl _gameBoardContent;

        public BoardControl GameBoardContent
        {
            get => _gameBoardContent;
            set
            {
                _gameBoardContent = value;
                OnPropertyChanged();
            }
        }

        private Visibility _countdownVisibility = Visibility.Visible;

        public Visibility CountdownVisibility
        {
            get => _countdownVisibility;
            set
            {
                _countdownVisibility = value;
                OnPropertyChanged();
            }
        }

        private String _countdownText = "Waiting...";

        public String CountdownText
        {
            get => _countdownText;
            set
            {
                _countdownText = value;
                OnPropertyChanged();
            }
        }

        private Visibility _roundResultVisibility = Visibility.Hidden;

        public Visibility RoundResultVisibility
        {
            get => _roundResultVisibility;
            set
            {
                _roundResultVisibility = value;
                OnPropertyChanged();
            }
        }

        private String _resultText;

        public String ResultText
        {
            get => _resultText;
            set
            {
                _resultText = value;
                OnPropertyChanged();
            }
        }

        private String _scoreText;

        public String ScoreText
        {
            get => _scoreText;
            set
            {
                _scoreText = value;
                OnPropertyChanged();
            }
        }

        private Visibility _gameResultVisibility = Visibility.Hidden;

        public Visibility GameResultVisibility
        {
            get => _gameResultVisibility;
            set
            {
                _gameResultVisibility = value;
                OnPropertyChanged();
            }
        }

        private CInstantCommand _nextRoundCommand;

        public CInstantCommand NextRoundCommand
        {
            get { return _nextRoundCommand ??= new CInstantCommand(NextRound); }
        }

        private CInstantCommand _leaveGameCommand;

        public CInstantCommand LeaveGameCommand
        {
            get { return _leaveGameCommand ??= new CInstantCommand(LeaveGame); }
        }

        private CInstantCommand _endGameCommand;

        public CInstantCommand EndGameCommand
        {
            get { return _endGameCommand ??= new CInstantCommand(EndGame); }
        }

        private void NextRound(Object data)
        {
            RoundResultVisibility = Visibility.Hidden;
            CountdownText = "Waiting...";
            CountdownVisibility = Visibility.Visible;
            GameBoardContent = new BoardControl(_gameId, _mySideColor);
        }

        public void ServerIsReady()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)LaunchCountdown);
        }

        public async void LaunchCountdown()
        {
            for (Int32 countdown = 3; countdown >= 1; countdown--)
            {
                CountdownText = countdown.ToString();
                await Task.Delay(1000);
            }
            CountdownVisibility = Visibility.Hidden;
        }

        public void EndRound(ESideColor winner, Boolean isLeave)
        {
            Panel.SetZIndex(GameBoardContent.Background, (Int32)EZIndex.EndGameBoard);
            switch (winner)
            {
                case ESideColor.White:
                    ShowResult(EPieceColor.White, isLeave);
                    break;
                case ESideColor.Black:
                    ShowResult(EPieceColor.Black, isLeave);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown or undefined color");
            }
        }

        public void LeaveGame(Object data)
        {
            _gameManagerService.Leave(_gameId, CAuthenticationStaff.Instance.User.UserId);
            EndGame(null);
        }

        private void EndGame(Object data)
        {
            CNavigator.Instance.NavigateToMainPage();
        }

        public void ShowResult(EPieceColor winner, Boolean isLeave)
        {
            if (!isLeave)
            {
                switch (winner)
                {
                    case EPieceColor.White:
                        ++WhitePlayerScore;
                        break;
                    case EPieceColor.Black:
                        ++BlackPlayerScore;
                        break;
                    default:
                        throw new Exception("Unknown or undefined color");
                }
            }
            ScoreText = WhitePlayerScore + " : " + BlackPlayerScore;
            if (isLeave)
            {
                ResultText = "Your opponent has left";
                RoundResultVisibility = Visibility.Hidden;
                GameResultVisibility = Visibility.Visible;
                return;
            }
            if (WhitePlayerScore == MaxScore || BlackPlayerScore == MaxScore)
            {
                ResultText = winner == _mySideColor ? "You won this game!" : "You lose this game!";
                GameResultVisibility = Visibility.Visible;
            }
            else
            {
                ResultText = winner == _mySideColor ? "You won!" : "You lose!";
                RoundResultVisibility = Visibility.Visible;
            }
        }
    }
}
