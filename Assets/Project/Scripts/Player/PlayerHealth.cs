using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;
    public event Action Died;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Debug.Log($"플레이어 피격: HP {currentHealth}/{maxHealth}");

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
    }
}
