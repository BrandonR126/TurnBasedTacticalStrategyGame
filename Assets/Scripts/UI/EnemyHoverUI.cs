using UnityEngine;

public class EnemyHoverUI : MonoBehaviour {
    Camera mainCamera;

    [SerializeField] HoverManager hoverManager;
    HitChanceUI hitChanceUI;
    UnitInfoUI unitInfoUI;
    [SerializeField] GameObject unitInfo;

    private UnitController currentHoveredUnit;
    private UnitController thisUnit;

    void Start() {
        hitChanceUI = GetComponent<HitChanceUI>();
        unitInfoUI = GetComponent<UnitInfoUI>();

        mainCamera = Camera.main;

        thisUnit = GetComponent<UnitController>();

        hoverManager.OnHoverEnter += HandleHoverEntry;
        hoverManager.OnHoverExit += HandleHoverExit;
    }

    private void HandleHoverEntry(UnitController unit) {
        currentHoveredUnit = hoverManager.currentHoveredUnit;

        if (currentHoveredUnit == thisUnit) {

            hitChanceUI.ShowHitChance(currentHoveredUnit);
            unitInfoUI.ShowUnitInfo(currentHoveredUnit);

        }
    }

    private void HandleHoverExit() {
        currentHoveredUnit = null;
        unitInfoUI.HideUnitInfo();
        hitChanceUI.HideHitChance();

    }
}
