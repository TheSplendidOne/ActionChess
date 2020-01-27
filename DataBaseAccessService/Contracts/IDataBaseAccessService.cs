using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataBaseAccessService
{
    [ServiceContract]
    public interface IDataBaseAccessService
    {
        [OperationContract]
        Boolean AddUser(CUser user);

        [OperationContract]
        Boolean AddGame(CGame game, params CBoard[] boards);

        [OperationContract]
        CUser FindUserById(Guid userId);

        [OperationContract]
        CUser FindUserByNickname(String nickname);

        [OperationContract]
        CUser FindRegisteredUser(String nickname);

        [OperationContract]
        List<CGame> FindGames(Guid userId);

        [OperationContract]
        CRecord FindRecord(Guid boardId);

        [OperationContract]
        CRecord GetRandomRecord();

        [OperationContract]
        Int32 GetWinRate(Guid userId);
    }
}
