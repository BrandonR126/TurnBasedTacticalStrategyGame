using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private HashSet<Tile> highlightedTiles = new HashSet<Tile>();

    #region Pathfinding for Player
    public void EnableReachableTiles(Vector2Int startPos, int maxMovement, float sprintValue) {
        ResetTiles();

        Dictionary<Vector2Int, int> reachable = Dijkstra(startPos, maxMovement, sprintValue);

        int sprintLimit = maxMovement + Mathf.FloorToInt(sprintValue);

        foreach (var kvp in reachable) {
            Vector2Int coords = kvp.Key;

            if (!gridManager.Grid.TryGetValue(coords, out GridNode node))
                continue;

            if (node.tile == null || !node.IsWalkable)
                continue;

            bool canReach = kvp.Value <= maxMovement;
            bool canSprint = kvp.Value <= sprintLimit;

            if (canReach) {
                node.tile.SetReachable(Color.blue);
            }

            else if (canSprint) {
                node.tile.SetSprintable(Color.yellow);
            }

            if (canReach || canSprint) {
                highlightedTiles.Add(node.tile);
            }
        }
    }
    #endregion

    #region Pathfinding for Enemy AI
    public struct TileInfo {
        public Vector2Int coords;

        public bool reachable;
        public bool sprintable;
    }

    public Dictionary<Vector2Int, TileInfo> ReturnReachableTiles(Vector2Int startPos, int maxMovement, float sprintValue) {
        ResetTiles();

        Dictionary<Vector2Int, int> reachable = Dijkstra(startPos, maxMovement, sprintValue);
        //Debug.Log("Reachable tiles count: " + reachable.Count);

        Dictionary<Vector2Int, TileInfo> result = new Dictionary<Vector2Int, TileInfo>();

        int sprintLimit = maxMovement + Mathf.FloorToInt(sprintValue);

        foreach (var kvp in reachable) {
            Vector2Int coords = kvp.Key;

            if (!gridManager.Grid.TryGetValue(coords, out GridNode node))
                continue;

            if (node.tile == null || !node.IsWalkable)
                continue;

            result[coords] = new TileInfo {
                coords = coords,

                reachable = kvp.Value <= maxMovement,
                sprintable = kvp.Value <= sprintLimit
            };
        }
        //Debug.Log("Processed reachable tiles count: " + result.Count);
        return result;
    }

    #endregion


    public void ResetTiles() {
        foreach (var node in gridManager.Grid.Values) {
            if (node.tile != null && (node.tile.isReachable || node.tile.isSprintable))
                node.tile.ResetPathfinding();
        }
        highlightedTiles.Clear();
    }


    #region dijkstra
    // Returns all reachable nodes and their cost from start
    public Dictionary<Vector2Int, int> Dijkstra(Vector2Int startPos, int maxMovement, float sprintValue) {
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>(); // all reaachable nodes and the cheapest way to get to them
        SimplePriorityQueue<Vector2Int, int> frontier = new SimplePriorityQueue<Vector2Int, int>();

        frontier.Enqueue(startPos, 0);
        costSoFar[startPos] = 0; // initial node costs nothing

        while (frontier.Count > 0) {
            Vector2Int current = frontier.Dequeue();

            if (!gridManager.TryGetNode(current, out var currentNode))
                continue;

            foreach (Vector2Int neighborPos in GetNeighbors(current, true)) {
                GridNode neighborNode = gridManager.GetNodeOrNull(neighborPos);

                if (neighborNode == null)
                    continue;

                int newCost = costSoFar[current] + neighborNode.movementCost;

                if (sprintValue == 0 && newCost > maxMovement) {
                    continue;
                }

                else if(sprintValue > 0 && newCost > maxMovement + sprintValue) {
                    continue;
                }

                if (!costSoFar.ContainsKey(neighborPos) || newCost < costSoFar[neighborPos]) {
                    costSoFar[neighborPos] = newCost;

                    if (frontier.Contains(neighborPos)) // queue the new node
                        frontier.UpdatePriority(neighborPos, newCost);
                    else
                        frontier.Enqueue(neighborPos, newCost);

                }
            }
        }


        return costSoFar;
    }
    #endregion

    #region A*
    // A* (Used to actually find route for unit to take)
    public List<GridNode> FindPath(Vector2Int startPos, Vector2Int endPos) {
        SimplePriorityQueue<Vector2Int, int> open = new SimplePriorityQueue<Vector2Int, int>(); // need to visit
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>(); // fully explored

        Dictionary<Vector2Int, int> cost = new Dictionary<Vector2Int, int>(); // total cost to reach node
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>(); // <child node, parent node>, child node being the node visited after parent

        cost[startPos] = 0;

        int heuristic = Chebyshev(startPos, endPos);

        open.Enqueue(startPos, heuristic);


        while (open.Count > 0) {
            Vector2Int currentPos = open.Dequeue();
            GridNode currentNode = gridManager.GetNode(currentPos);

            if (currentPos == endPos) {
                return ReconstructPath(cameFrom, currentPos);
            }

            closed.Add(currentPos);

            foreach(Vector2Int neighbor in GetNeighbors(currentPos, true)) {
                if (closed.Contains(neighbor) || !gridManager.IsWalkable(neighbor)) { // if node fully explored or node is occupied, skip
                    continue;
                }

                int potentialCost = cost[currentPos] + gridManager.Grid[neighbor].movementCost; // get potential cost to use in future calcs

                if (!cost.ContainsKey(neighbor) || potentialCost < cost[neighbor]) { // if never been to node or cost is less than last cheapest, change cost 
                    cost[neighbor] = potentialCost;
                    int newCost = potentialCost + Chebyshev(neighbor, endPos);

                    if (open.Contains(neighbor)) {
                        open.UpdatePriority(neighbor, newCost);
                    }
                    else {
                        open.Enqueue(neighbor, newCost);
                    }
;
                    cameFrom[neighbor] = currentPos;
                }
            }

        }
        Debug.LogError("Could not find path!"); // never reached goal
        return null;

    }

    int Chebyshev(Vector2Int startPos, Vector2Int endPos) {
        int dx = Mathf.Abs(startPos.x - endPos.x);
        int dy = Mathf.Abs(startPos.y - endPos.y);
        return Mathf.Max(dx, dy);
    }

    List<GridNode> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current) {
        List<GridNode> path = new List<GridNode>();
        while (cameFrom.ContainsKey(current)) { // follows cameFrom from endPos to startPos
            path.Add(gridManager.GetNode(current)); // add current node to path
            current = cameFrom[current]; // update current to value of parent before the last (moves to parent node)
        }
        path.Add(gridManager.GetNode(current)); // Add start node
        path.Reverse(); // reverse list (makes it start -> back, not back -> front)
        return path;
    }
    #endregion


    // 8-directional neighbors
    private List<Vector2Int> GetNeighbors(Vector2Int pos, bool needDiag) {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int right = pos + Vector2Int.right;
        Vector2Int left = pos + Vector2Int.left;
        Vector2Int up = pos + Vector2Int.up;
        Vector2Int down = pos + Vector2Int.down;

        // Orthogonal
        if (gridManager.IsWalkable(right)) neighbors.Add(right);
        if (gridManager.IsWalkable(left)) neighbors.Add(left);
        if (gridManager.IsWalkable(up)) neighbors.Add(up);
        if (gridManager.IsWalkable(down)) neighbors.Add(down);

        if (!needDiag)
            return neighbors;

        // Diagonals (with corner-cutting prevention)
        Vector2Int upRight = pos + Vector2Int.up + Vector2Int.right;
        if (gridManager.IsWalkable(upRight) &&
            gridManager.IsWalkable(up) &&
            gridManager.IsWalkable(right)) {
            neighbors.Add(upRight);
        }

        Vector2Int upLeft = pos + Vector2Int.up + Vector2Int.left;
        if (gridManager.IsWalkable(upLeft) &&
            gridManager.IsWalkable(up) &&
            gridManager.IsWalkable(left)) {
            neighbors.Add(upLeft);
        }

        Vector2Int downRight = pos + Vector2Int.down + Vector2Int.right;
        if (gridManager.IsWalkable(downRight) &&
            gridManager.IsWalkable(down) &&
            gridManager.IsWalkable(right)) {
            neighbors.Add(downRight);
        }

        Vector2Int downLeft = pos + Vector2Int.down + Vector2Int.left;
        if (gridManager.IsWalkable(downLeft) &&
            gridManager.IsWalkable(down) &&
            gridManager.IsWalkable(left)) {
            neighbors.Add(downLeft);
        }

        return neighbors;
    }



}
