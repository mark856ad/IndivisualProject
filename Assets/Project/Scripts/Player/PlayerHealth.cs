using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    public event Action Died;
    public event Action<float, float> HealthChanged;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Debug.Log($"플레이어 피격: HP {currentHealth}/{maxHealth}");
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            isDead = true;
            Debug.Log("플레이어 사망");
            Died?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"플레이어 회복: HP {currentHealth}/{maxHealth}");
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void Kill()
    {
        if (isDead) return;

        currentHealth = 0f;
        HealthChanged?.Invoke(currentHealth, maxHealth);

        isDead = true;
        Debug.Log("플레이어 압사");
        Died?.Invoke();
    }
}
