using UnityEngine;
using UnityEngine.UI;

public class HandleSelected : MonoBehaviour {
    GridManager gridManager;
    Pathfinder pathfinder;
    SelectedUnit selectedUnit;
    TileClickHandler tileClickHandler;
    Renderer rend;
    UnitController unitController;
    TeamController teamController;
    UnitAttack unitAttack;

    [SerializeField] GameObject selectAttackToggle;

    private void Awake() {
        tileClickHandler = GetComponent<TileClickHandler>();
        rend = GetComponentInChildren<Renderer>();
        unitController = GetComponent<UnitController>();
        teamController = GetComponentInParent<TeamController>();
        unitAttack = GetComponent<UnitAttack>();
    }

    private void Start() {
        gridManager = GameContext.Instance.GridManager;
        pathfinder = GameContext.Instance.Pathfinder;
        selectedUnit = GameContext.Instance.SelectedUnit;
    }


    public void Selected() {
        unitController.isSelected = true;
        unitAttack.AddTargetListener();

        selectedUnit.SetSelectedUnit(unitController);

        Highlight(true, rend);

        tileClickHandler.enabled = true;

        float sprintForPathfinder;

        if (teamController.availableAP - unitController.sprintCost >= 0) {
            sprintForPathfinder = unitController.sprintAmount;
        }
        else {
            sprintForPathfinder = 0;
        }

        pathfinder.EnableReachableTiles(
        gridManager.WorldToGrid(this.transform.position), unitController.currentMobility, sprintForPathfinder);
    }

    public void Deselected() {
        unitController.isSelected = false;
        unitAttack.RemoveTargetListener();

        selectedUnit.DeselectUnit();

        Highlight(false, rend);
        tileClickHandler.enabled = false;

        pathfinder.ResetTiles();
    }

    public void Highlight(bool selected, Renderer rend) { // just for testing - will be in own script eventully
        if (selected) {
            rend.material.color = Color.yellow;
            selectAttackToggle.SetActive(true);
        }
        else {
            rend.material.color = Color.deepSkyBlue;
            selectAttackToggle.SetActive(false);
        }
    }
}
