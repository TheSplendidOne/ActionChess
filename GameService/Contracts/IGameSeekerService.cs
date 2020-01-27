using System;
using System.ServiceModel;

namespace GameService
{
    [ServiceContract(CallbackContract = typeof(IGameSeekerCallback))]
    public interface IGameSeekerService
    {
        [OperationContract(IsOneWay = true)]
        void StartSearchGame(Guid playerId);

        [OperationContract(IsOneWay = true)]
        void CancelSearchGame(Guid playerId);
    }
}
