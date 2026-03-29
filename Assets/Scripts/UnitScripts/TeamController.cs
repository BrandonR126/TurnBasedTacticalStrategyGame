using UnityEngine;
using System.Collections.Generic;

public class TeamController : MonoBehaviour
{
    [SerializeField] PlayerTurnUI playerTurnUI;

    public int turnMaxAP;

    private int _availableAP;

    public int availableAP {
        get { return _availableAP; }
        set {
            _availableAP = value;
            if (playerTurnUI != null) {
                playerTurnUI.UpdateApUI(_availableAP, turnMaxAP);
            }
        }
    }


    private void OnEnable() {

        if (TurnManager.Instance != null) {
            TurnManager.Instance.OnPlayerTurnStarted += StartPlayerTurn;
            TurnManager.Instance.OnPlayerTurnEnded += PlayerEndedTurn;
        }
        else {
            Debug.LogError("TurnManager instance is null in TeamController.OnEnable");
        }
    }


    private void OnDisable() {
        TurnManager.Instance.OnPlayerTurnStarted -= StartPlayerTurn;
        TurnManager.Instance.OnPlayerTurnEnded -= PlayerEndedTurn;
    }

    private List<UnitController> units = new List<UnitController>();

    public void RegisterUnit(UnitController unit) {
        units.Add(unit);
    }

    public void UnregisterUnit(UnitController unit) {
        units.Remove(unit);
    }

    public void LowerAp(int amount) {
        availableAP -= amount;
    }

    private void StartPlayerTurn() {
        turnMaxAP = 0;

        foreach (var unit in units) {
            unit.StartUnitTurn();
            turnMaxAP += unit.unitAP;
        }

        availableAP = turnMaxAP;

        if (playerTurnUI != null) { // player team controller
             playerTurnUI.EnablePlayerUI();
        }
    }

    private void PlayerEndedTurn() {
        if (playerTurnUI != null) { // player team controller
            playerTurnUI.DisablePlayerUI();
        }
        return;
    }


}
