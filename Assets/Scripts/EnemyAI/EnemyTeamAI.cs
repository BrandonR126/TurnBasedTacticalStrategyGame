using AI.Controllers;
using System.Collections.Generic;
using UnityEngine;
using AI.Data;
using AI.Evaluation;
using System.Collections;

public class EnemyTeamAI : MonoBehaviour
{
    TurnManager turnManager;
    EnemyAI enemyAI;
    TeamController teamController;

    private UnitController[] enemyUnits = new UnitController[] { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnManager = TurnManager.Instance;
        teamController = GetComponent<TeamController>();

        turnManager.OnEnemyTurnStarted += StartEnemyTurn;
        turnManager.OnEnemyTurnEnded += EnemyTurnEnded;
    }

    private void StartEnemyTurn() {
        enemyUnits = GetComponentsInChildren<UnitController>(false);

        Debug.Log("Enemy turn started. Enemy team is taking their actions.");

        StartCoroutine(CompleteBestTurn());
    }

    private IEnumerator CompleteBestTurn() {
        int highestValue = 0;
        int currentAP = teamController.availableAP;
        EnemyAI bestAI = null;
        TurnDecision bestTurn = null;

        Debug.Log(currentAP + " AP available for enemy team.");

        foreach (UnitController enemy in enemyUnits) {
            if (!enemy.isActiveAndEnabled || !enemy) {
                continue;
            }

            if(currentAP <= 0 && enemy.currentMobility == 0) {
                continue;
            }

            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if(!ai) {
                continue;
            }

            TurnDecision aiBestTurn = ai.FindBestTurn();

            int potentialAPCost = currentAP - aiBestTurn.action.cost;

            if(potentialAPCost < 0) {
                Debug.Log($"Skipping {enemy.name} - best turn costs {aiBestTurn.action.cost} AP, but only {currentAP} AP left.");
                continue;
            }

            if (aiBestTurn != null && aiBestTurn.turnValue > highestValue) {
                highestValue = aiBestTurn.turnValue;
                bestAI = ai;
                bestTurn = aiBestTurn;
            }
        }

        if (bestAI == null || bestTurn == null || highestValue <= 0) {
            Debug.Log("Enemy team has no valid actions. Ending turn.");
            turnManager.RequestEndTurn();

            yield break;
        }

        yield return StartCoroutine(bestAI.ContinueTurn(bestTurn));

        teamController.availableAP -= bestTurn.action.cost;
        if (teamController.availableAP <= 0) {
            Debug.Log("Enemy team has no AP left. Ending turn.");
            turnManager.RequestEndTurn();
        }
        else { // do turn again, ap left
            yield return new WaitForSeconds(0.5f); // small delay between actions for readability
            StartCoroutine(CompleteBestTurn());
        }
    }

    private void EnemyTurnEnded() {
        Debug.Log("Enemy turn ended. Player's turn is starting.");
    }
}
