using System;

namespace GameService
{
    internal class CPieceRemovedEntry : CRecordEntry
    {
        public CPieceRemovedEntry(Int32 pieceId) : base(pieceId, DateTime.UtcNow)
        {
        }
    }
}
