using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

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

        if (currentHealth <= 0f)
        {
            isDead = true;
            Debug.Log("플레이어 사망 (임무 실패 처리는 나중에 연결)");
        }
    }
}
