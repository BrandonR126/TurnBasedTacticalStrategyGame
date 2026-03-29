using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoverManager : MonoBehaviour
{
    public LayerMask unitLayer;

    public UnitController currentHoveredUnit;

    public event Action<UnitController> OnHoverEnter;
    public event Action OnHoverExit;

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            if (((1 << hit.transform.gameObject.layer) & unitLayer) == 0) { // takes layer that ray hit, unit layer, ands them (both layers need to be same to not be 0)


                if (currentHoveredUnit != null) {
                    ClearHover();
                }

                return;
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer)) {
            UnitController unit = hit.transform.GetComponent<UnitController>();

            if (unit != currentHoveredUnit) {
                currentHoveredUnit = unit;
                OnHoverEnter?.Invoke(unit);
            }
        }
    }

    public void ClearHover() {
        currentHoveredUnit = null;
        OnHoverExit?.Invoke();
    }
}
