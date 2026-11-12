using UnityEngine;

public class MissionStats : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private StratagemInventory inventory;
    [SerializeField] private MissionController mission;

    public int KillCount { get; private set; }
    public int StratagemUses { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public event System.Action<bool> StatsReady;
    public bool Success { get; private set; }
    private float startTime;
    private bool running;

    private void OnEnable()
    {
        spawner.EnemyKilled += OnEnemyKilled;
        inventory.StateChanged += OnStratagemUsed;
        mission.MissionEnded += OnMissionEnded;

        startTime = Time.time;
        running = true;
    }

    private void OnDisable()
    {
        spawner.EnemyKilled -= OnEnemyKilled;
        inventory.StateChanged -= OnStratagemUsed;
        mission.MissionEnded -= OnMissionEnded;
    }

    private void Update()
    {
        if (running)
        {
            ElapsedSeconds = Time.time - startTime;
        }
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        KillCount++;
    }

    private void OnStratagemUsed(StratagemData data)
    {
        StratagemUses++;
    }

    private void OnMissionEnded(bool success)
    {
        running = false;
        ElapsedSeconds = Time.time - startTime;
        Success = success;

        Debug.Log($"결과: {(success ? "성공" : "실패")}, 시간 {ElapsedSeconds:F0}초, 처치 {KillCount}, 스트라타잼 {StratagemUses}회");
        StatsReady?.Invoke(success);
    }
}