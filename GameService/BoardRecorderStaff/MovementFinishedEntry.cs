using System;
using Animator;

namespace GameService
{
    internal class CMovementFinishedEntry : CRecordEntry
    {
        public CPoint To { get; }

        public CMovementFinishedEntry(Int32 pieceId, CPoint to) : base(pieceId, DateTime.UtcNow)
        {
            To = to;
        }
    }
}
