using System;
using System.Windows.Controls;
using SharedServiceLibrary;

namespace ActionChessClient
{
    internal partial class BoardRecordPlayerControl : UserControl, IDisposable
    {
        public BoardRecordPlayerControl(CMainPageViewModel mainPageViewModel, CBoardRecordData boardRecord, Boolean isAutoBoardRecordPlayer)
        {
            InitializeComponent();
            DataContext = new CBoardRecordPlayerControlPresenter(mainPageViewModel, boardRecord, this, isAutoBoardRecordPlayer);
        }

        public void Dispose()
        {
            ((CBoardRecordPlayerControlPresenter)DataContext).Dispose();
        }
    }
}
