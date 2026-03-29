using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ClickManager : MonoBehaviour {
    public static ClickManager Instance; // Singleton

    public ISelectable selectedObject;

    AttackManager attackManager;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start() {
        attackManager = GameContext.Instance.AttackManager;
    }

    public void HandleLeftClick(InputAction.CallbackContext context) { // sees if where user clicked has IClickable, and if so, calls it
        if (!context.performed) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if(Physics.Raycast(ray, out RaycastHit hit)) {
            if(hit.transform.TryGetComponent<IClickable>(out var clickable)) {
                clickable.OnClick();
            }
        }


    }

    public void Select(ISelectable obj) { // deselects last object if needed, selects new object
        if(selectedObject == obj) {
            Deselect();
            return;
        }
        if (obj is UnitController unit && unit.team == Team.Enemy) {
            if (attackManager.attacking) {
                attackManager.Targeted(unit);

                return;
            }
            else return;
        }
        

        Deselect();
        selectedObject = obj;
        selectedObject.Selected();
    }

    public void Deselect() {
        if (selectedObject != null) {
            selectedObject.Deselected();
            selectedObject = null;
        }
    }
}
