using System;
using System.Linq;

namespace ActionChessClient
{
    internal class CMovementValidator
    {
        private readonly MovementRule[] _rules;

        public CMovementValidator(params MovementRule[] ruleParts)
        {
            _rules = ruleParts;
        }

        public Boolean CheckAll(CBoardOccupancy boardStates, CTile currentTile, CTile proposedTile)
        {
            return _rules.Any(rule =>
            {
                try
                {
                    return rule(boardStates, currentTile, proposedTile);
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
