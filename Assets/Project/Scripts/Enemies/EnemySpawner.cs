using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private WaveData wave;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int prewarmPerType = 5;
    private readonly HashSet<Enemy> alive = new HashSet<Enemy>();
    private bool rallyActive;
    private Vector3 rallyPoint;

    public event System.Action<Enemy> EnemyKilled;
    public float Pressure { get; private set; }

    private readonly Dictionary<Enemy, SimplePool<Enemy>> poolsByPrefab
        = new Dictionary<Enemy, SimplePool<Enemy>>();
    private readonly Dictionary<Enemy, SimplePool<Enemy>> poolOfInstance
        = new Dictionary<Enemy, SimplePool<Enemy>>();

    private int aliveCount;
    private float lastSpawnTime;
    private float startTime;
private float missionPressure;    

    private void Awake()
    {
        if (wave == null || wave.entries.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogError("Wave 또는 Spawn Points가 비어 있습니다.", this);
            enabled = false;
            return;
        }

        foreach (WaveEntry entry in wave.entries)
        {
            if (poolsByPrefab.ContainsKey(entry.prefab)) continue;

            poolsByPrefab.Add(entry.prefab,
                new SimplePool<Enemy>(entry.prefab, prewarmPerType, $"{entry.prefab.name} Pool"));
        }
    }

    private void OnEnable()
    {
        startTime = Time.time;
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        float timePressure = Mathf.Clamp01((Time.time - startTime) / wave.timeToMaxPressure);
        Pressure = Mathf.Max(timePressure, missionPressure);

        float interval = Mathf.Lerp(wave.startInterval, wave.minInterval, Pressure);
        int maxAlive = Mathf.RoundToInt(Mathf.Lerp(wave.startMaxAlive, wave.endMaxAlive, Pressure));

        if (aliveCount >= maxAlive) return;
        if (Time.time - lastSpawnTime < interval) return;

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

        Enemy prefab = PickPrefab();
        SimplePool<Enemy> pool = poolsByPrefab[prefab];

        Enemy enemy = pool.Get(hit.position, Quaternion.identity);
        poolOfInstance[enemy] = pool;
        alive.Add(enemy);

        if (rallyActive && enemy.TryGetComponent(out EnemyAI ai))
        {
            ai.SetRallyPoint(rallyPoint);
        }
        enemy.Died += OnEnemyDied;
        aliveCount++;
    }

    private Enemy PickPrefab()
    {
        float total = 0f;
        foreach (WaveEntry entry in wave.entries) total += entry.weight;

        float roll = Random.value * total;

        foreach (WaveEntry entry in wave.entries)
        {
            roll -= entry.weight;
            if (roll <= 0f) return entry.prefab;
        }

        return wave.entries[wave.entries.Length - 1].prefab;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.Died -= OnEnemyDied;
        alive.Remove(enemy);
        poolOfInstance[enemy].Release(enemy);
        aliveCount--;

        EnemyKilled?.Invoke(enemy);
    }
    public void SetMissionPressure(float value)
    {
        missionPressure = Mathf.Clamp01(value);
    }
    public void SetRallyPoint(Vector3 point)
    {
        if (NavMesh.SamplePosition(point, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            point = hit.position;
        }

        rallyActive = true;
        rallyPoint = point;

        foreach (Enemy enemy in alive)
        {
            if (enemy.TryGetComponent(out EnemyAI ai))
            {
                ai.SetRallyPoint(rallyPoint);
            }
        }

        Debug.Log($"집결 지점 설정: 살아 있는 적 {alive.Count}마리");
    }
}
