using System;

namespace GameService
{
    internal abstract class CRecordEntry
    {
        public Int32 PieceId { get; }

        public DateTime EntryCreated { get; }

        protected CRecordEntry(Int32 pieceId, DateTime entryCreated)
        {
            PieceId = pieceId;
            EntryCreated = entryCreated;
        }
    }
}
