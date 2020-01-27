using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{
    [ServiceContract(CallbackContract = typeof(IGameManagerCallback))]
    public interface IGameManagerService
    {
        [OperationContract(IsOneWay = true)]
        void ConnectGameManager(Guid gameId, Guid playerId);

        [OperationContract(IsOneWay = true)]
        void Leave(Guid gameId, Guid playerId);
    }
}
