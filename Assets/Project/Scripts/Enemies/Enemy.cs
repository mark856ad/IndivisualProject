using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;

    public event Action<Enemy> Died;

    private float currentHealth;
    private bool isDead;

    private void OnEnable()
    {
        currentHealth = data.maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{data.enemyName} 피격: 남은 HP {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Died?.Invoke(this);
    }
    public void Kill()
    {
        if (isDead) return;

        currentHealth = 0f;
        Die();
    }
}