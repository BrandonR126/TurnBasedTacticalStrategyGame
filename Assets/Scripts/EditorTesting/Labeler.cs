using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Labeler : MonoBehaviour
{
    // labels the invidividual tiles, both in game and in editor hierarchy. for debugging only.

    TextMeshPro label;
    public Vector2Int cords = new Vector2Int();
    private GridManager gridManager;


    private Vector3 lastPos;

    private void Awake() {
        label = GetComponentInChildren<TextMeshPro>();
        gridManager = FindAnyObjectByType<GridManager>();

        UpdateLabel();
    }

    private void Update() {
        if (transform.position != lastPos) {
            UpdateLabel();
            lastPos = transform.position;
        }
    }

    private void UpdateLabel() {
        if (gridManager == null) return; 
        cords = gridManager.WorldToGrid(transform.position);
        if (label != null) label.text = $"({cords.x}, {cords.y})";
        transform.name = cords.ToString();
    }
}

