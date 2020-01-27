using System;
using Animator;

namespace GameService
{
    internal class CMovementBeganEntry : CRecordEntry
    {
        public CPoint From { get; }

        public CMovementBeganEntry(Int32 pieceId, CPoint from) : base(pieceId, DateTime.UtcNow)
        {
            From = from;
        }
    }
}
