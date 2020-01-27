using System;
using System.ServiceModel;

namespace GameService
{
    internal class CPlayer
    {
        public OperationContext GameSeekerContext { get; }

        public OperationContext GameManagerContext { get; set; }

        public OperationContext VirtualBoardContext { get; set; }

        public Guid PlayerId { get; }

        public EConnectionState BoardConnectionState { get; set; }

        public ESideColor? SideColor { get; set; }

        public CPlayer(Guid playerId, OperationContext gameSeekerContext)
        {
            PlayerId = playerId;
            GameSeekerContext = gameSeekerContext;
            BoardConnectionState = EConnectionState.Waiting;
        }
    }
}
