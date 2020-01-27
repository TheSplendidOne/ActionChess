using System;
using System.ServiceModel;

namespace GameService
{
    [ServiceContract]
    internal interface IGameManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ServerIsReady();

        [OperationContract(IsOneWay = true)]
        void EndRound(ESideColor winner, Boolean isLeave);
    }
}
