using System;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Objective[] objectives;
    [SerializeField] private EnemySpawner spawner;

    public event Action AllCompleted;

    private int completedCount;

    private void OnEnable()
    {
        foreach (Objective objective in objectives)
        {
            objective.Completed += OnObjectiveCompleted;
            objective.ProgressChanged += OnProgressChanged;
        }
    }

    private void OnDisable()
    {
        foreach (Objective objective in objectives)
        {
            objective.Completed -= OnObjectiveCompleted;
            objective.ProgressChanged -= OnProgressChanged;
        }
    }

    private void OnProgressChanged(Objective objective)
    {
        UpdatePressure();
    }

    private void UpdatePressure()
    {
        if (spawner == null) return;

        float highest = 0f;
        foreach (Objective o in objectives)
        {
            if (o.IsCompleted) continue;
            highest = Mathf.Max(highest, o.Progress);
        }

        Debug.Log($"압박 갱신: {highest:F2}");
        spawner.SetMissionPressure(highest);
    }

    private void OnObjectiveCompleted(Objective objective)
    {
        completedCount++;
        Debug.Log($"목표 완료: {objective.Title} ({completedCount}/{objectives.Length})");

        UpdatePressure();

        if (completedCount >= objectives.Length)
        {
            Debug.Log("모든 목표 완료 (탈출 지점 활성화는 11회차)");
            AllCompleted?.Invoke();
        }
    }
}
