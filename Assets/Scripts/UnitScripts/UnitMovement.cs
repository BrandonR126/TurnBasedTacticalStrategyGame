using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private GridManager gridManager;
    UnitController unitController;

    private Queue<GridNode> moveQueue;

    public float moveSpeed = 2f;
    public float sprintSpeed = 4f;
    public float rotationSpeed = 5f;

    public static bool unitMoving = false;

    [SerializeField] float nodeArrivalThreshold = 0.05f;

    private GridNode currentNode;

    private void Awake() {
        unitController = GetComponent<UnitController>();
    }

    private void Start() {
        gridManager = GameContext.Instance.GridManager;
    }

    private void Update() {
        if ((unitController.currentState != UnitController.UnitState.Walking && unitController.currentState != UnitController.UnitState.Sprinting) || moveQueue == null) {
            return;
        }

        MoveAlongPath();
    }


    public void StartMovement(List<GridNode> path) {
        unitMoving = true;
        moveQueue = new Queue<GridNode>(path);
        currentNode = moveQueue.Dequeue(); // pop off first node; it is the current node
    }

    private void MoveAlongPath() {
        if (moveQueue == null || moveQueue.Count == 0) return;

        GridNode targetNode = moveQueue.Peek();
        Vector3 targetPos = gridManager.GridToWorld(targetNode.coords);
        targetPos.y = transform.position.y;

        // Move
        float speed = (unitController.currentState == UnitController.UnitState.Sprinting) ? sprintSpeed : moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Rotate
        Vector3 direction = targetPos - transform.position;
        if (direction.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if arrived
        if (Vector3.Distance(transform.position, targetPos) < nodeArrivalThreshold) {
            if (currentNode != null) currentNode.unitOnNode = null; // remove from old node
            currentNode = moveQueue.Dequeue();
            currentNode.unitOnNode = unitController;

            if (moveQueue.Count == 0) {
                unitController.SetState(UnitController.UnitState.Idle);

                if(unitController.team == Team.Player) {
                    GameContext.Instance.Pathfinder.EnableReachableTiles(gridManager.WorldToGrid(unitController.transform.position), unitController.currentMobility, unitController.sprintAmount);
                }

                unitMoving = false;
            }
        }
    }

}
