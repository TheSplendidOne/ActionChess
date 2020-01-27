using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServiceLibrary
{
    public class CPawnTransformedData
    {
        public Int32 PieceId { get; }

        public Boolean IsWhite { get; }

        public CPawnTransformedData(Int32 pieceId, Boolean isWhite)
        {
            PieceId = pieceId;
            IsWhite = isWhite;
        }
    }
}
