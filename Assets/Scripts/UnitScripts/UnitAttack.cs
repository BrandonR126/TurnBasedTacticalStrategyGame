using UnityEngine;
using UnityEngine.UI;

public class UnitAttack : MonoBehaviour
{
    AttackManager attackManager;
    TeamController teamController;
    UnitController unit;

    [SerializeField] Toggle toggleAttack;

    private void Start() {
        attackManager = GameContext.Instance.AttackManager;
        teamController = GetComponentInParent<TeamController>();
        unit = GetComponent<UnitController>();;
    }

    public void AddTargetListener() {
        attackManager.AddTargetListener(OnAttack);
    }

    public void RemoveTargetListener() {
        attackManager.RemoveTargetListener(OnAttack);
    }

    private void OnAttack(UnitController target) {
        int attackCost = unit.attackCost;

        if (!unit.isSelected || teamController.availableAP - attackCost < 0) return;

        StartCoroutine(attackManager.Attack(unit, target));

        teamController.LowerAp(attackCost);

        toggleAttack.isOn = false;
    }

    public void sendAttack(UnitController target) {
        target.healthManager.TakeDamage(unit.attackDamage);
    }
}
