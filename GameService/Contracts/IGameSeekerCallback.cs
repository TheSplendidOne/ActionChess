using System;
using System.ServiceModel;

namespace GameService
{
    [ServiceContract]
    public interface IGameSeekerCallback
    {
        [OperationContract(IsOneWay = true)]
        void CreateGame(Guid gameId, Guid opponentId, ESideColor mySideColor);
    }
}
