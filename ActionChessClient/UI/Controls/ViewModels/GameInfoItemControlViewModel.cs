using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SharedServiceLibrary;
using Newtonsoft.Json;

namespace ActionChessClient
{
    internal class CGameInfoItemControlViewModel : CMainPageControlViewModel
    {
        private static readonly Int32 MaxBoardsCount = 3;

        private readonly List<CBoardModel> _boardModels;

        public CGameInfoItemControlViewModel(CMainPageViewModel mainPageViewModel, CGameInfoModel gameInfoModel)
        {
            ParentViewModel = mainPageViewModel;
            WhitePlayerNickname = gameInfoModel.WhitePlayerNickname;
            BlackPlayerNickname = gameInfoModel.BlackPlayerNickname;
            WhitePlayerScore = gameInfoModel.WhitePlayerScore;
            BlackPlayerScore = gameInfoModel.BlackPlayerScore;
            WinOrDefeat = gameInfoModel.IsWin ? "Win" : "Lose";
            _boardModels = GetStructuredBoardModels(gameInfoModel.BoardModels);
            Int32 boardNumber = 0;
            FirstBoardButtonOpacity = GetOpacity(_boardModels[boardNumber++]);
            SecondBoardButtonOpacity = GetOpacity(_boardModels[boardNumber++]);
            ThirdBoardButtonOpacity = GetOpacity(_boardModels[boardNumber]);
        }

        private static List<CBoardModel> GetStructuredBoardModels(List<CBoardModel> boardModels)
        {
            List<CBoardModel> newList = new List<CBoardModel>();
            for(Int32 boardNumber = 0; boardNumber < MaxBoardsCount; ++boardNumber)
                newList.Add(boardModels.FirstOrDefault(board => board.BoardNumber == boardNumber));
            return newList;
        }

        private static Double GetOpacity(CBoardModel boardModel)
        {
            return (Double)Application.Current.Resources[
                boardModel != null
                    ? "ActiveBoardOpacity"
                    : "InactiveBoardOpacity"];
        }

        private String _whitePlayerNickname;

        public String WhitePlayerNickname
        {
            get => _whitePlayerNickname;
            set
            {
                _whitePlayerNickname = value;
                OnPropertyChanged();
            }
        }

        private String _blackPlayerNickname;

        public String BlackPlayerNickname
        {
            get => _blackPlayerNickname;
            set
            {
                _blackPlayerNickname = value;
                OnPropertyChanged();
            }
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

        private String _winOrDefeat;

        public String WinOrDefeat
        {
            get => _winOrDefeat;
            set
            {
                _winOrDefeat = value;
                OnPropertyChanged();
            }
        }

        private Double _firstBoardButtonOpacity;

        public Double FirstBoardButtonOpacity
        {
            get => _firstBoardButtonOpacity;
            set
            {
                _firstBoardButtonOpacity = value;
                OnPropertyChanged();
            }
        }

        private Double _secondBoardButtonOpacity;

        public Double SecondBoardButtonOpacity
        {
            get => _secondBoardButtonOpacity;
            set
            {
                _secondBoardButtonOpacity = value;
                OnPropertyChanged();
            }
        }

        private Double _thirdBoardButtonOpacity;

        public Double ThirdBoardButtonOpacity
        {
            get => _thirdBoardButtonOpacity;
            set
            {
                _thirdBoardButtonOpacity = value;
                OnPropertyChanged();
            }
        }

        private CInstantCommand _getBoardRecordCommand;
        public CInstantCommand GetBoardRecordCommand
        {
            get { return _getBoardRecordCommand ??= new CInstantCommand(GetBoardRecord); }
        }

        private void GetBoardRecord(Object boardNumber)
        {
            CBoardModel model = _boardModels[Int32.Parse((String) boardNumber)];
            if (model != null)
            {
                ParentViewModel.ActualBoardRecord.Dispose();
                ParentViewModel.ActualBoardRecord = new BoardRecordPlayerControl(ParentViewModel,
                    JsonConvert.DeserializeObject<CBoardRecordData>(SDbService.Get().FindRecord(
                        model.BoardId).Record), false);
            }
        }
    }
}
