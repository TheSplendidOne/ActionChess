using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ServiceModel;
using DataBaseAccessService;
using Newtonsoft.Json;
using SharedServiceLibrary;
using NLog;

namespace GameService
{
    internal class CServerGame
    {
        public Guid GameId { get; }

        public CPlayer WhitePlayer { get; }

        public CPlayer BlackPlayer { get; }

        public CBoardSynchronizer CurrentBoard { get; private set; }

        private readonly List<CBoard> _finishedBoards;

        private readonly Action<Guid> _endgameAction;

        private readonly DateTime _startGameTime;

        private CBoardRecorder _currentRecorder;

        private Int32 _whiteScore;

        private Int32 _blackScore;

        private static readonly Int32 MaxScore = 2;

        public CServerGame(Guid gameId, CPlayer whitePlayer, CPlayer blackPlayer, Action<Guid> endgameAction)
        {
            GameId = gameId;
            WhitePlayer = whitePlayer;
            WhitePlayer.SideColor = ESideColor.White;
            BlackPlayer = blackPlayer;
            BlackPlayer.SideColor = ESideColor.Black;
            _endgameAction = endgameAction;
            _finishedBoards = new List<CBoard>();
            _startGameTime = DateTime.UtcNow;
        }

        private CPlayer GetPlayer(Guid playerId)
        {
            if (WhitePlayer.PlayerId == playerId)
                return WhitePlayer;
            if (BlackPlayer.PlayerId == playerId)
                return BlackPlayer;
            throw new Exception("Unknown player");
        }

        private Guid GetPlayerId(ESideColor sideColor)
        {
            return sideColor switch
            {
                ESideColor.White => WhitePlayer.PlayerId,
                ESideColor.Black => BlackPlayer.PlayerId,
                _ => throw new Exception($"Unknown color: {sideColor}")
            };
        }

        private CPlayer GetOpponentPlayer(Guid playerId)
        {
            if (WhitePlayer.PlayerId == playerId)
                return BlackPlayer;
            if (BlackPlayer.PlayerId == playerId)
                return WhitePlayer;
            throw new Exception("Unknown player");
        }

        public void ConnectGameManager(Guid playerId)
        {
            CPlayer connectingPlayer = GetPlayer(playerId);
            connectingPlayer.GameManagerContext = OperationContext.Current;
        }

        public void ConnectBoard(Guid playerId)
        {
            CPlayer connectingPlayer = GetPlayer(playerId);
            connectingPlayer.VirtualBoardContext = OperationContext.Current;
            connectingPlayer.BoardConnectionState = EConnectionState.Connected;
            if (WhitePlayer.BoardConnectionState == EConnectionState.Connected &&
                BlackPlayer.BoardConnectionState == EConnectionState.Connected)
            {
                _currentRecorder = new CBoardRecorder();
                CurrentBoard = new CBoardSynchronizer(WhitePlayer, BlackPlayer, _currentRecorder, ProcessBoardResult);
                WhitePlayer.GameManagerContext.GetCallbackChannel<IGameManagerCallback>().ServerIsReady();
                BlackPlayer.GameManagerContext.GetCallbackChannel<IGameManagerCallback>().ServerIsReady();
            }
        }

        public void Leave(Guid playerId)
        {
            if (CurrentBoard != null)
                ResetBoard();
            CPlayer winner = GetOpponentPlayer(playerId);
            winner.GameManagerContext.GetCallbackChannel<IGameManagerCallback>().EndRound((ESideColor)winner.SideColor, true);
            EndGame(winner.PlayerId);
        }

        private void ProcessBoardResult(ESideColor winner)
        {
            ResetBoard();
            WhitePlayer.BoardConnectionState = BlackPlayer.BoardConnectionState = EConnectionState.Waiting;
            WhitePlayer.GameManagerContext.GetCallbackChannel<IGameManagerCallback>().EndRound(winner, false);
            BlackPlayer.GameManagerContext.GetCallbackChannel<IGameManagerCallback>().EndRound(winner, false);
            if (winner == ESideColor.White)
                ++_whiteScore;
            else
                ++_blackScore;
            if (_whiteScore == MaxScore || _blackScore == MaxScore)
                EndGame(GetPlayerId(winner));
        }

        private void EndGame(Guid winner)
        {
            SDbService.Get().AddGame(new CGame
            {
                GameId = GameId,
                WhitePlayerId = WhitePlayer.PlayerId,
                BlackPlayerId = BlackPlayer.PlayerId,
                StartGameTime = _startGameTime,
                EndGameTime = DateTime.UtcNow,
                WhiteScore = _whiteScore,
                BlackScore = _blackScore,
                WinnerId = winner,
            }, _finishedBoards.ToArray());
            _endgameAction.Invoke(GameId);
        }

        private void ResetBoard()
        {
            CBoardSynchronizer temp = CurrentBoard;
            CurrentBoard = null;
            temp.Dispose();
            AddFinishedBoard(temp);
            _currentRecorder = null;
        }

        private void AddFinishedBoard(CBoardSynchronizer board)
        {
            Guid newBoardGuid = Guid.NewGuid();
            _finishedBoards.Add(new CBoard
            {
                BoardId = newBoardGuid,
                GameId = GameId,
                BoardNumber = _finishedBoards.Count,
                Record = new CRecord()
                {
                    BoardId = newBoardGuid,
                    Record = JsonConvert.SerializeObject(_currentRecorder.GetBoardRecordData(board))
                }
            });
        }
    }
}
