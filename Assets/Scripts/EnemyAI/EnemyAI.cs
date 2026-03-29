using System.Collections.Generic;
using UnityEngine;
using AI.Data;
using AI.Evaluation;
using System.Collections;

namespace AI.Controllers {

    public class EnemyAI : MonoBehaviour {
        FindBestMove findBestMove;
        UnitMovement unitMovement;
        UnitController unitController;
        AttackManager attackManager;


        private void Start() {
            findBestMove = GetComponent<FindBestMove>();
            unitController = GetComponent<UnitController>();
            unitMovement = GetComponent<UnitMovement>();
            attackManager = GameContext.Instance.AttackManager;
        }



        public TurnDecision FindBestTurn() {
            TurnDecision turnToTake = findBestMove.FindBestTurn(Vector2Int.RoundToInt(transform.position), unitController.currentMobility, unitController.sprintAmount);

            return turnToTake;
        }

        public IEnumerator ContinueTurn(TurnDecision turn) {

            if(turn.routeToTile != null && turn.routeToTile.Count > 1) {
                unitController.SetState(turn.movementState);
                unitMovement.StartMovement(turn.routeToTile);

                yield return new WaitUntil(() => unitController.currentState == UnitController.UnitState.Idle);
            }

            if (turn.routeToTile.Count <= 1) {
                Debug.Log("Staying in place this turn.");
            }

            if (turn.action != null && turn.action.type == TurnActionType.Attack) {
                ExecuteAttack(turn.action);
            }
        }

        private void ExecuteAttack(TurnAction action) {
            action.damage = 10; // Placeholder damage value for testing. will be replaced with actual damage calculation logic in the future.

            // Placeholder for future attack execution logic
            if (action.type == TurnActionType.Attack) {
                StartCoroutine(attackManager.Attack(unitController, action.target));

                Debug.Log($"Attacking {action.target.name} for {action.damage} damage!");
            }

            else {
                Debug.Log("Attack no work :(");
            }
        }
    }
}
