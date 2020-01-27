using System;
using System.ServiceModel;
using Animator;

namespace GameService
{
    [ServiceContract]
    public interface IVirtualBoardCallback
    {
        [OperationContract(IsOneWay = true)]
        void MovePiece(Int32 pieceId, CPoint newPosition);

        [OperationContract(IsOneWay = true)]
        void HandleCollision(Int32 invaderId, Int32 capturedId, Boolean isCollisionStopsMoving);

        [OperationContract(IsOneWay = true)]
        void TransformPawnToQueen(Int32 pieceId);
    }
}
