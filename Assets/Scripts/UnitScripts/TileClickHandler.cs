using System.Collections.Generic;
using UnityEngine;
using static UnitController;

public class TileClickHandler : MonoBehaviour
{
    UnitController unit;
    GridManager gridManager;
    Pathfinder pathfinder;
    UnitMovement unitMovement;
    TeamController teamController;

    private void Awake() {
        unit = GetComponent<UnitController>();
        unitMovement = GetComponent<UnitMovement>();
        teamController = GetComponentInParent<TeamController>();
    }
    private void Start() {
        gridManager = GameContext.Instance.GridManager;
        pathfinder = GameContext.Instance.Pathfinder;
    }

    private void OnEnable() {
        Tile.OnTileClick += HandleTileClick;
    }

    private void OnDisable() {
        Tile.OnTileClick -= HandleTileClick;
    }

    #region handle clicking on tile
    void HandleTileClick(Tile tile) {
        if (unit.isSelected == false) { 
            return;
        }
        
        if (UnitMovement.unitMoving)
            return;

        if (!tile.isReachable && !tile.isSprintable)
            return;

        bool isSprint = tile.isSprintable && !tile.isReachable;

        if (isSprint) {
            if (teamController.availableAP < unit.sprintCost) {
                return;
            }
            unit.currentMobility = Mathf.FloorToInt(unit.currentMobility + unit.sprintAmount);
            unit.sprintAmount = 0;
        }


        List<GridNode> pathToTile = pathfinder.FindPath(
            gridManager.WorldToGrid(unit.transform.position),
            gridManager.WorldToGrid(tile.transform.position)
        );

        if (pathToTile == null || pathToTile.Count == 0) return;

        // Check mobility
        int mobilityCost = 0;

        for (int i = 1; i < pathToTile.Count; i++) {
            mobilityCost += pathToTile[i].movementCost;
        }

        if (unit.currentMobility < mobilityCost)
            return;


        if (isSprint)
        {
            unit.SetState(UnitState.Sprinting);

            teamController.LowerAp(unit.sprintCost);

            unit.currentMobility -= mobilityCost;
        }


        else {
            unit.SetState(UnitState.Walking);
            unit.currentMobility -= mobilityCost;
        }

        unitMovement.StartMovement(pathToTile);
    }
    #endregion
}
