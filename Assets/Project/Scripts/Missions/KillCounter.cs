using UnityEngine;

public class KillCounter : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    private int killCount;

    private void OnEnable()
    {
        spawner.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        spawner.EnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        killCount++;
        Debug.Log($"처치 수: {killCount}");
    }
}