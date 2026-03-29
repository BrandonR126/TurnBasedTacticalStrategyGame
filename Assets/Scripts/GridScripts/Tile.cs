using System;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable {
    public GridNode node; // assigned by GridManager

    private Renderer rend;

    public static event Action<Tile> OnTileClick;

    public bool isReachable = false;
    public bool isSprintable = false;
    
    [SerializeField] bool movementBlocked;
    public bool IsOccupied {
        get => node != null && node.movementBlocked;
        set {
            if (node == null) return;
            node.movementBlocked = value;
        }
    }

    private void Awake() {
        rend = GetComponentInChildren<Renderer>();
    }

    public void InitializeNode() {
        node.movementBlocked = movementBlocked;
    }

    public void OnClick() {
        OnTileClick?.Invoke(this);
    }

    public void SetReachable(Color color) {
        isReachable = true;
        
        rend.material.color = color;
    }

    public void SetSprintable(Color color) {
        isSprintable = true;

        rend.material.color = color;
    }

    public void ResetPathfinding() {
        isReachable = false;
        isSprintable = false;

        ResetColor();
    }

    public void ResetColor() {
        rend.material.color = new Color32(76, 168, 60, 255);
    }

    [ContextMenu("Toggle Occupied")]
    void ToggleOccupied() {
        IsOccupied = !IsOccupied;
    }

}
