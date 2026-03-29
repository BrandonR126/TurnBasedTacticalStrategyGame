using UnityEngine;
using TMPro;

public class HitChanceUI : MonoBehaviour
{
    AttackManager attackManager;
    SelectedUnit selectedUnit;

    [SerializeField] TextMeshProUGUI hitChanceText;

    private void Start() {
        attackManager = GameContext.Instance.AttackManager;

        selectedUnit = GameContext.Instance.SelectedUnit;
    }

    public void ShowHitChance(UnitController hoveredUnit) {
        if(selectedUnit.selectedUnit == null) {
            hitChanceText.gameObject.SetActive(false);
            return;
        }

        int hitChance = attackManager.CheckChanceToHitFromUnit(selectedUnit.selectedUnit, hoveredUnit);

        hitChanceText.text = "Hit Chance: " + hitChance + "%";

        hitChanceText.gameObject.SetActive(true);
    }

    public void HideHitChance() {
        hitChanceText.gameObject.SetActive(false);
    }
}
