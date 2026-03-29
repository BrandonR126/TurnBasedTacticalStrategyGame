using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject hoverUI;
    [SerializeField] TextMeshProUGUI attackCostUI;
    [SerializeField] TextMeshProUGUI attackDamageUI;

    public void OnPointerEnter(PointerEventData eventData) {
        UnitController selectedUnit = GameContext.Instance.SelectedUnit.selectedUnit;

        hoverUI.SetActive(true);
        attackDamageUI.text = $"Attack Damage: {selectedUnit.attackDamage}";
        attackCostUI.text = $"Attack Cost: {selectedUnit.attackCost}";
    }
    public void OnPointerExit(PointerEventData eventData) {
        hoverUI.SetActive(false);
    }
} 
