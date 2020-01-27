using System;
using System.Collections.Generic;
using System.Linq;
using Animator;
using Newtonsoft.Json;
using SharedServiceLibrary;
using NLog;

namespace GameService
{
    internal class CBoardRecorder
    {
        private readonly List<CRecordEntry> _entries;

        private readonly List<CMovementFinishedEntry> _movementFinishedEntries;

        public CBoardRecorder()
        {
            _entries = new List<CRecordEntry>();
            _movementFinishedEntries = new List<CMovementFinishedEntry>();
        }

        public void AddEntry(CRecordEntry entry)
        {
            lock (_entries)
            {
                _entries.Add(entry);
            }
        }

        public void AddMovementFinishedEntry(CMovementFinishedEntry movementFinishedEntry)
        {
            lock (_movementFinishedEntries)
            {
                _movementFinishedEntries.Add(movementFinishedEntry);
            }
        }

        public CBoardRecordData GetBoardRecordData(CBoardSynchronizer board)
        {
            List<CSerializedRecordAtom> record = new List<CSerializedRecordAtom>();
            DateTime previousEntryTime = default;
            lock (_entries)
            {
                foreach (CRecordEntry recordEntry in _entries)
                {
                    if (previousEntryTime != default)
                        AddWaitData(recordEntry);
                    previousEntryTime = recordEntry.EntryCreated;
                    switch (recordEntry)
                    {
                        case CMovementBeganEntry movementBeganEntry:
                            AddMovementData(movementBeganEntry);
                            break;
                        case CPieceRemovedEntry pieceRemovedEntry:
                            AddPieceRemovedData(pieceRemovedEntry);
                            break;
                        case CPawnTransformedEntry pawnTransformedEntry:
                            AddPawnTransformedData(pawnTransformedEntry);
                            break;
                        default:
                            throw new ArgumentException($"Unknown type: {recordEntry.GetType()}");
                    }
                }
            }

            void AddMovementData(CMovementBeganEntry current)
            {
                lock (_movementFinishedEntries)
                {
                    CMovementFinishedEntry correspondingMovementFinishedEntry =
                        _movementFinishedEntries.FirstOrDefault(entry => entry.PieceId == current.PieceId && entry.EntryCreated > current.EntryCreated);
                    CPoint toPoint = correspondingMovementFinishedEntry?.To ?? board.GetPieceById(current.PieceId).Position;
                    CMovementData movementData = new CMovementData(current.PieceId, current.From, toPoint);
                    record.Add(new CSerializedRecordAtom(EAtomType.Movement, JsonConvert.SerializeObject(movementData)));
                }
            }

            void AddPieceRemovedData(CPieceRemovedEntry current)
            {
                record.Add(new CSerializedRecordAtom(EAtomType.PieceRemoved,
                    JsonConvert.SerializeObject(new CPieceRemovedData(current.PieceId))));
            }

            void AddPawnTransformedData(CPawnTransformedEntry current)
            {
                record.Add(new CSerializedRecordAtom(EAtomType.PawnTransformed,
                    JsonConvert.SerializeObject(new CPawnTransformedData(current.PieceId, current.IsWhite))));
            }

            void AddWaitData(CRecordEntry current)
            {
                record.Add(new CSerializedRecordAtom(EAtomType.Wait,
                    JsonConvert.SerializeObject(new CWaitData((current.EntryCreated - previousEntryTime).Duration()))));
            }
            return new CBoardRecordData(record);
        }
    }
}