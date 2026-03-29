using System.Collections.Generic;

namespace AI.Data {
    public class TurnDecision {
        public Pathfinder.TileInfo moveTile;
        public List<GridNode> routeToTile;
        public UnitController.UnitState movementState;

        public TurnAction action;

        public int turnValue;
        public int apRemaining;
    }
}