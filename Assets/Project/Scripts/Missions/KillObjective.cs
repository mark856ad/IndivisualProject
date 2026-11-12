using UnityEngine;

public class KillObjective : Objective
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private int targetCount = 20;

    private int killCount;

    public override string Title => $"EXTERMINATE {targetCount} ENEMIES";
    public override float Progress => Mathf.Clamp01((float)killCount / targetCount);
    public int KillCount => killCount;
    public int TargetCount => targetCount;
    public override string ProgressText => $"{killCount}/{targetCount}";

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
        NotifyProgress();

        if (killCount >= targetCount)
        {
            Complete();
        }
    }
}