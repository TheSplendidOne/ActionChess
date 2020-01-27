using System;

namespace ActionChessClient
{
    internal delegate Boolean MovementRule(CBoardOccupancy boardStates, CTile currentTile, CTile proposedTile);
}
