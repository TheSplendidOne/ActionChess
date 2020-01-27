using System;
using System.Collections.Generic;

namespace ActionChessClient
{
    internal class CGameInfoModel
    {
        public String WhitePlayerNickname { get; set; }

        public String BlackPlayerNickname { get; set; }

        public Int32 WhitePlayerScore { get; set; }

        public Int32 BlackPlayerScore { get; set; }

        public Boolean IsWin { get; set; }

        public List<CBoardModel> BoardModels { get; set; }
    }
}
