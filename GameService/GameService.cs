using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Animator;
using NLog;

namespace GameService
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CGameService : IGameSeekerService, IGameManagerService, IVirtualBoardService
    {
        //public static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Int32 MaxSeekers = 2;

        private readonly List<CPlayer> _gameSeekers = new List<CPlayer>(MaxSeekers);

        private readonly ConcurrentDictionary<Guid, (CServerGame, Object)> _gamesAndLockers = new ConcurrentDictionary<Guid, (CServerGame, Object)>();

        private readonly Object _seekersLocker = new Object();

        private readonly SemaphoreSlim _seekersCounter = new SemaphoreSlim(MaxSeekers, MaxSeekers);

        private readonly AutoResetEvent _launchGameEnter = new AutoResetEvent(false);

        private readonly AutoResetEvent _launchGameReturn = new AutoResetEvent(false);

        public CGameService()
        {
            Task.Factory.StartNew(LaunchGame);
        }

        public void StartSearchGame(Guid playerId)
        {
            CPlayer user = new CPlayer(playerId, OperationContext.Current);
            _seekersCounter.Wait();
            lock (_seekersLocker)
            {
                _gameSeekers.Add(user);
                if (_seekersCounter.CurrentCount == 0)
                    WaitHandle.SignalAndWait(_launchGameEnter, _launchGameReturn);
            }
        }

        public void CancelSearchGame(Guid playerId)
        {
            lock (_seekersLocker)
            {
                CPlayer seeker = _gameSeekers.FirstOrDefault(s => s.PlayerId == playerId);
                if (seeker != null)
                {
                    _gameSeekers.Remove(seeker);
                    _seekersCounter.Release();
                }
            }
        }

        private void LaunchGame()
        {
            while (true)
            {
                _launchGameEnter.WaitOne();
                CreateGame(_gameSeekers[0], _gameSeekers[1]);
                _gameSeekers.Clear();
                _seekersCounter.Release(2);
                _launchGameReturn.Set();
            }
        }

        private void CreateGame(CPlayer whitePlayer, CPlayer blackPlayer)
        {
            Guid gameId = Guid.NewGuid();
            _gamesAndLockers.TryAdd(gameId, (new CServerGame(gameId, whitePlayer, blackPlayer, RemoveGame), new Object()));
            whitePlayer.GameSeekerContext.GetCallbackChannel<IGameSeekerCallback>().CreateGame(gameId, blackPlayer.PlayerId, ESideColor.White);
            blackPlayer.GameSeekerContext.GetCallbackChannel<IGameSeekerCallback>().CreateGame(gameId, whitePlayer.PlayerId, ESideColor.Black);
        }

        public void ConnectGameManager(Guid gameId, Guid playerId)
        {
            if (_gamesAndLockers.TryGetValue(gameId, out (CServerGame, Object) pair))
            {
                lock (pair.Item2)
                {
                    pair.Item1.ConnectGameManager(playerId);
                }
            }
        }

        public void ConnectBoard(Guid gameId, Guid playerId)
        {
            if (_gamesAndLockers.TryGetValue(gameId, out (CServerGame, Object) pair))
            {
                lock (pair.Item2)
                {
                    pair.Item1.ConnectBoard(playerId);
                }
            }
        }

        public void Leave(Guid gameId, Guid playerId)
        {
            if (_gamesAndLockers.TryGetValue(gameId, out (CServerGame, Object) pair))
            {
                lock (pair.Item2)
                {
                    if(_gamesAndLockers.ContainsKey(gameId))
                        pair.Item1.Leave(playerId);
                }
            }
        }

        public void TryMovePiece(Guid gameId, Int32 pieceId, CPoint newPosition)
        {
            if (!_gamesAndLockers.ContainsKey(gameId)) return;
            if (_gamesAndLockers.TryGetValue(gameId, out (CServerGame, Object) pair))
            {
                lock (pair.Item2)
                {
                    pair.Item1.CurrentBoard?.MovePiece(pieceId, newPosition);
                }
            }
        }

        private void RemoveGame(Guid boardId)
        {
            _gamesAndLockers.TryRemove(boardId, out (CServerGame, Object) pair);
        }
    }
}
