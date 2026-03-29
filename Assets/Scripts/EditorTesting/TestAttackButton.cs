using UnityEngine;

public class TestAttackButton : MonoBehaviour
{
    public HealthManager target;

    public int damage;

    [ContextMenu("Deal Damage")]
    public void Attack() {
        if (target != null) {
            target.TakeDamage(damage);
        }
    }
}
