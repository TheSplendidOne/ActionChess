using System;
using System.Windows;
using DataBaseAccessService;
using Newtonsoft.Json;
using SharedServiceLibrary;

namespace ActionChessClient
{
    internal class CMainPageViewModel : CViewModelBase
    {
        private UIElement _actualControl;

        public UIElement ActualControl
        {
            get => _actualControl;
            set
            {
                _actualControl = value;
                OnPropertyChanged();
            }
        }

        private BoardRecordPlayerControl _actualBoardRecord;

        public BoardRecordPlayerControl ActualBoardRecord
        {
            get => _actualBoardRecord;
            set
            {
                _actualBoardRecord = value;
                OnPropertyChanged();
            }
        }

        public CMainPageViewModel()
        {
            SetRandomBoardRecord();
        }

        public void SetRandomBoardRecord()
        {
            String record = SDbService.Get().GetRandomRecord()?.Record;
            if (record != null)
                ActualBoardRecord = new BoardRecordPlayerControl(this,
                    JsonConvert.DeserializeObject<CBoardRecordData>(record), true);
        }
    }
}
