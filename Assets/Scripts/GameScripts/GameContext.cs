using System.Runtime.CompilerServices;
using UnityEngine;

public class GameContext : MonoBehaviour {
    public static GameContext Instance { get; private set; }

    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private AttackManager attackManager;
    [SerializeField] private SelectedUnit selectedUnit;

    public GridManager GridManager => gridManager;
    public Pathfinder Pathfinder => pathfinder;
    public AttackManager AttackManager => attackManager;
    public SelectedUnit SelectedUnit => selectedUnit;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Assert(gridManager != null, "GridManager not assigned");
        Debug.Assert(pathfinder != null, "Pathfinder not assigned");
        Debug.Assert(attackManager != null, "AttackManager not assigned");
    }
}
