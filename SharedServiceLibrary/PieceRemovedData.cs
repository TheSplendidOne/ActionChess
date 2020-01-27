using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServiceLibrary
{
    public class CPieceRemovedData
    {
        public Int32 PieceId { get; set; }

        public CPieceRemovedData(Int32 pieceId)
        {
            PieceId = pieceId;
        }
    }
}
