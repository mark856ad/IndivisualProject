using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int prewarmCount = 5;
    [SerializeField] private int maxAlive = 5;
    [SerializeField] private float spawnInterval = 3f;

    private SimplePool<Enemy> pool;
    private int aliveCount;
    private float lastSpawnTime;
    public event System.Action<Enemy> EnemyKilled;

    private void Awake()
    {
        pool = new SimplePool<Enemy>(enemyPrefab, prewarmCount, "Enemy Pool");
    }

    private void Update()
    {
        if (aliveCount >= maxAlive) return;
        if (Time.time - lastSpawnTime < spawnInterval) return;

        lastSpawnTime = Time.time;
        Spawn();
    }

    private void Spawn()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (!NavMesh.SamplePosition(point.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            return;
        }

        Enemy enemy = pool.Get(hit.position, Quaternion.identity);
        enemy.Died += OnEnemyDied;
        aliveCount++;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.Died -= OnEnemyDied;
        pool.Release(enemy);
        aliveCount--;

        EnemyKilled?.Invoke(enemy);
    }
}
