using System;
using System.Collections.Generic;

namespace ActionChessClient
{
    internal class CProfileModel
    {
        public String Nickname { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Int32 GamesCount { get; set; }

        public Int32 WinRate { get; set; }

        public List<CGameInfoModel> GamesInfo { get; set; }
    }
}
