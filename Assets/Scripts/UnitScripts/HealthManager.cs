using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{

    public int maxHealth;
    [SerializeField] private int currentHealth;

    public UnityEvent<int> onDamaged = new UnityEvent<int>();
    public UnityEvent onDeath = new UnityEvent();

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount) {
        currentHealth -= amount;
        onDamaged.Invoke(amount);

        if (currentHealth <= 0) {
            Die();
        }
    }

    public int GetHealth() {
        return currentHealth;
    }

    public void Die() {
        onDeath.Invoke();
    }
}
