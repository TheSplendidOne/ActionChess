using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using SharedServiceLibrary;
using ActionChessClient.GameService;
using DataBaseAccessService;

namespace ActionChessClient
{
    internal class CMenuControlViewModel : CMainPageControlViewModel, IGameSeekerServiceCallback
    {
        private Boolean _seekGame;

        private GameSeekerServiceClient _gameSeekerServiceClient;

        private GameSeekerServiceClient GameSeekerServiceClient
        {
            get { return _gameSeekerServiceClient ??= new GameSeekerServiceClient(new InstanceContext(this)); }
        }

        private String _greeting = "Hi, " + CAuthenticationStaff.Instance.User.Nickname;

        public String Greeting
        {
            get => _greeting;
            set
            {
                _greeting = value;
                OnPropertyChanged();
            }
        }

        private String _playButtonText = "Play";

        public String PlayButtonText
        {
            get => _playButtonText;
            set
            {
                _playButtonText = value;
                OnPropertyChanged();
            }
        }

        private CInstantCommand _playOrCancelCommand;

        public CInstantCommand PlayOrCancelCommand
        {
            get { return _playOrCancelCommand ??= new CInstantCommand(PlayOrCancel); }
        }

        private CInstantCommand _onMouseEnterCommand;

        public CInstantCommand OnMouseEnterCommand
        {
            get { return _onMouseEnterCommand ??= new CInstantCommand(OnMouseEnter); }
        }

        private CInstantCommand _onMouseLeaveCommand;

        public CInstantCommand OnMouseLeaveCommand
        {
            get { return _onMouseLeaveCommand ??= new CInstantCommand(OnMouseLeave); }
        }

        private CInstantCommand _showProfileCommand;

        public CInstantCommand ShowProfileCommand
        {
            get { return _showProfileCommand ??= new CInstantCommand(TryShowProfile); }
        }

        private CInstantCommand _logOutCommand;

        public CInstantCommand LogOutCommand
        {
            get { return _logOutCommand ??= new CInstantCommand(LogOut); }
        }

        private void PlayOrCancel(Object data)
        {
            if(_seekGame)
                CancelSeekGame();
            else
                TryPlay();
        }

        private void TryPlay()
        { 
            GameSeekerServiceClient.StartSearchGame(CAuthenticationStaff.Instance.User.UserId);
            _seekGame = true;
            PlayButtonText = "Cancel";
        }

        private void CancelSeekGame()
        {
            GameSeekerServiceClient.CancelSearchGame(CAuthenticationStaff.Instance.User.UserId);
            _seekGame = false;
            PlayButtonText = "Play";
        }

        private void OnMouseEnter(Object data)
        {
            if (_seekGame)
                PlayButtonText = "Cancel";
        }

        private void OnMouseLeave(Object data)
        {
            if (_seekGame)
                PlayButtonText = "Waiting...";
        }

        private void TryShowProfile(Object data)
        {
            List<CGame> games = SDbService.Get().FindGames(CAuthenticationStaff.Instance.User.UserId);
            CProfileModel profileModel = new CProfileModel
            {
                Nickname = CAuthenticationStaff.Instance.User.Nickname,
                RegistrationDate = CAuthenticationStaff.Instance.User.RegistrationDate,
                GamesCount = games.Count,
                WinRate = (Int32)(games.Count > 0
                    ? (Double)games.Count(game => game.WinnerId == CAuthenticationStaff.Instance.User.UserId) / games.Count * 100 // Percent value
                    : 0),
                GamesInfo = new List<CGameInfoModel>(games
                    .OrderByDescending(game => game.StartGameTime)
                    .Select(game => new CGameInfoModel
                    {
                        WhitePlayerNickname = game.WhitePlayer.Nickname,
                        BlackPlayerNickname = game.BlackPlayer.Nickname,
                        WhitePlayerScore = game.WhiteScore,
                        BlackPlayerScore = game.BlackScore,
                        IsWin = game.WinnerId == CAuthenticationStaff.Instance.User.UserId,
                        BoardModels = new List<CBoardModel>(game.Boards.Select(board => new CBoardModel
                        {
                            BoardId = board.BoardId,
                            BoardNumber = board.BoardNumber
                        }))
                    }))
            };
            if (_seekGame)
                CancelSeekGame();
            ParentViewModel.ActualControl = new ProfileControl(ParentViewModel, profileModel);
        }

        private void LogOut(Object data)
        {
            if(_seekGame)
                CancelSeekGame();
            CAuthenticationStaff.Instance.Client.LogOutAsync(CAuthenticationStaff.Instance.User.UserId);
            ParentViewModel.ActualControl = new LogInControl(ParentViewModel);
            CAuthenticationStaff.Instance.User = null;
        }

        public void CreateGame(Guid gameId, Guid opponentId, ESideColor mySideColor)
        {
            switch (mySideColor)
            {
                case ESideColor.White:
                    CNavigator.Instance.NavigateToGamePage(
                        new CGameStartModel(gameId, opponentId, EPieceColor.White));
                    break;
                case ESideColor.Black:
                    CNavigator.Instance.NavigateToGamePage(
                        new CGameStartModel(gameId, opponentId, EPieceColor.Black));
                    break;
                default:
                    throw new InvalidEnumArgumentException(@"Unknown or undefined color");
            }
        }
    }
}
