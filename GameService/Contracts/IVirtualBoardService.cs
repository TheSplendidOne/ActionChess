using System;
using System.ServiceModel;
using Animator;

namespace GameService
{
    [ServiceContract(CallbackContract = typeof(IVirtualBoardCallback))]
    public interface IVirtualBoardService
    {
        [OperationContract(IsOneWay = true)]
        void ConnectBoard(Guid gameId, Guid playerId);

        [OperationContract(IsOneWay = true)]
        void TryMovePiece(Guid gameId, Int32 pieceId, CPoint newPosition);
    }
}
