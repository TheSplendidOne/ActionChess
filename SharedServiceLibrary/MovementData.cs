using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animator;

namespace SharedServiceLibrary
{
    public class CMovementData
    {
        public Int32 PieceId { get; set; }

        public CPoint From { get; set; }

        public CPoint To { get; set; }

        public CMovementData(Int32 pieceId, CPoint from, CPoint to)
        {
            PieceId = pieceId;
            From = from;
            To = to;
        }
    }
}
