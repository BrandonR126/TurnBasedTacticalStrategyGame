using UnityEngine;

public class PlayerHoverUI : MonoBehaviour
{
    Camera mainCamera;

    [SerializeField] HoverManager hoverManager;
    UnitInfoUI unitInfoUI;
    [SerializeField] GameObject unitInfo;

    private UnitController currentHoveredUnit;
    private UnitController thisUnit;

    void Start() {
        unitInfoUI = GetComponent<UnitInfoUI>();

        mainCamera = Camera.main;

        thisUnit = GetComponent<UnitController>();

        hoverManager.OnHoverEnter += HandleHoverEntry;
        hoverManager.OnHoverExit += HandleHoverExit;
    }

    private void HandleHoverEntry(UnitController unit) {
        currentHoveredUnit = hoverManager.currentHoveredUnit;

        if (currentHoveredUnit == thisUnit) {

            unitInfoUI.ShowUnitInfo(currentHoveredUnit);

        }
    }

    private void HandleHoverExit() {
        currentHoveredUnit = null;

        unitInfoUI.HideUnitInfo();
    }
}
