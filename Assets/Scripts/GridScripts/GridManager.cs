using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour {
    [SerializeField] Vector2Int minGrid;
    [SerializeField] Vector2Int maxGrid;
    [SerializeField] int unityGridSize;   // Scale
    [SerializeField] bool visualizeGrid = false;

    public Dictionary<Vector2Int, GridNode> Grid { get; private set; }
    public int UnityGridSize => unityGridSize;

    private Tile[] allTiles;

    private void Awake() {
        // Create all nodes
        Grid = new Dictionary<Vector2Int, GridNode>();
        for (int x = minGrid.x; x <= maxGrid.x; x++) {
            for (int y = minGrid.y; y <= maxGrid.y; y++) {
                Vector2Int coords = new Vector2Int(x, y);
                Grid.Add(coords, new GridNode(coords));

                if (visualizeGrid) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = GridToWorld(coords);
                    cube.transform.localScale = Vector3.one * 0.9f;
                    cube.GetComponent<Renderer>().material.color = Color.white;
                    cube.name = $"DebugTile_{x}_{y}";
                }
            }
        }
    }

    private void Start() {
        // Find all Tiles in the scene and assign them to nodes
        allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in allTiles) {
            Vector2Int coords = WorldToGrid(tile.transform.position);

            if (Grid.TryGetValue(coords, out GridNode node)) {
                tile.node = node;
                node.tile = tile;

                tile.InitializeNode();
            }
            else {
                Debug.LogWarning("No GridNode found at " + coords + " for tile " + tile.name);
            }
        }
    }

    public GridNode GetNode(Vector2Int nodePos) {
        return Grid[nodePos];
    }

    public bool TryGetNode(Vector2Int pos, out GridNode node) {
        return Grid.TryGetValue(pos, out node);
    }

    public GridNode GetNodeOrNull(Vector2Int pos) {
        Grid.TryGetValue(pos, out var node);
        return node;
    }

    public Vector3 GridToWorld(Vector2Int gridPos) {
        return new Vector3(gridPos.x * unityGridSize, 0f, gridPos.y * unityGridSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos) {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x / unityGridSize), Mathf.RoundToInt(worldPos.z / unityGridSize));
    }

    public bool IsWalkable(Vector2Int pos) {
        return TryGetNode(pos, out var node) && node.tile != null && node.IsWalkable;
    }

}
