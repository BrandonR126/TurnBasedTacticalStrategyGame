using UnityEngine;

// Individual node on grid
public class GridNode {
    public Vector2Int coords;

    public Tile tile;
    public UnitController unitOnNode;

    public int movementCost = 1;
    public bool movementBlocked = false;
    public bool HasUnit => unitOnNode != null;
    public bool IsWalkable => !movementBlocked && !HasUnit;

    public GridNode(Vector2Int coords) {
        this.coords = coords;


    }
}
