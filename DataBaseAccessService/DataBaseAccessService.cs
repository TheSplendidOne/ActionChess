using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ServiceModel;
using System.Linq;

namespace DataBaseAccessService
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CDataBaseAccessService : IDataBaseAccessService
    {
        private static T Invoke<T>(CDbOperationInvoker<T> invoker)
        {
            using (CActionChessDbContext dbContext = new CActionChessDbContext())
            {
                return invoker.Invoke(dbContext);
            }
        }

        public Boolean AddUser(CUser user)
        {
            return Invoke(new CDbOperationInvoker<Boolean>((context) =>
            {
                context.Users.Add(user);
                context.Authentications.Add(user.Authentication);
                context.SaveChanges();
                return true;
            }));
        }

        public Boolean AddGame(CGame game, params CBoard[] boards)
        {
            return Invoke(new CDbOperationInvoker<Boolean>((context) =>
            {
                context.Games.Add(game);
                foreach (CBoard board in boards)
                {
                    context.Boards.Add(board);
                    context.Records.Add(board.Record);
                    context.SaveChanges();
                }
                return true;
            }));
        }

        public CUser FindUserById(Guid userId)
        {
            return Invoke(new CDbOperationInvoker<CUser>((context) => context.Users.Find(userId)));
        }

        public CUser FindUserByNickname(String nickname)
        {
            return Invoke(new CDbOperationInvoker<CUser>((context) => context.Users.FirstOrDefault(user => user.Nickname == nickname)));
        }

        public CUser FindRegisteredUser(String nickname)
        {
            return Invoke(new CDbOperationInvoker<CUser>((context) =>
            {
                CUser entry = context.Users.FirstOrDefault(user => user.Nickname == nickname);
                if (entry == null) return null;
                context.Entry(entry).Reference("Authentication").Load();
                return entry;
            }));
        }

        public List<CGame> FindGames(Guid userId)
        {
            return Invoke(new CDbOperationInvoker<List<CGame>>
                ((context) => context.Games
                .Where(game => userId == game.WhitePlayerId || userId == game.BlackPlayerId)
                .Include(game => game.Boards)
                .Include(game => game.WhitePlayer)
                .Include(game => game.BlackPlayer)
                .ToList()));
        }

        public CRecord FindRecord(Guid boardId)
        {
            return Invoke(new CDbOperationInvoker<CRecord>((context) => context.Records.Find(boardId)));
        }

        public CRecord GetRandomRecord()
        {
            return Invoke(new CDbOperationInvoker<CRecord>((context) =>
            {
                if (context.Records.Any())
                {
                    Int32 boardNumber = new Random().Next(0, context.Records.Count());
                    return context.Records.OrderBy(record => record.BoardId).Skip(boardNumber).Take(1).Single();
                }

                return null;
            }));
        }

        public Int32 GetWinRate(Guid userId)
        {
            return Invoke(new CDbOperationInvoker<Int32>
            ((context) =>
            { 
                Int32 totalGamesCount = context.Games.Count(game => userId == game.WhitePlayerId || userId == game.BlackPlayerId);
                Int32 wonGamesCount = context.Games.Count(game => game.WinnerId == userId);
                return (Int32)(totalGamesCount > 0
                    ? (Double)wonGamesCount / totalGamesCount * 100 // Percent value
                    : 0);
            }));
        }
    }
}