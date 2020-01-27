using System;

namespace GameService
{
    internal class CPawnTransformedEntry : CRecordEntry
    {
        public Boolean IsWhite { get; }

        public CPawnTransformedEntry(Int32 pieceId, Boolean isWhite) : base(pieceId, DateTime.UtcNow)
        {
            IsWhite = isWhite;
        }
    }
}
