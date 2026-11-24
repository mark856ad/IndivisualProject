using System;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    [SerializeField] private EscapeSequence escape;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private InputModeController modeController;
    [SerializeField] private EnemySpawner spawner;

    public event Action<bool> MissionEnded;
    public bool IsEnded { get; private set; }

    private void OnEnable()
    {
        escape.MissionSucceeded += OnSucceeded;
        escape.MissionFailed += OnFailed;
        playerHealth.Died += OnFailed;
    }

    private void OnDisable()
    {
        escape.MissionSucceeded -= OnSucceeded;
        escape.MissionFailed -= OnFailed;
        playerHealth.Died -= OnFailed;

        Time.timeScale = 1f;
    }

    private void OnSucceeded() { End(true); }
    private void OnFailed() { End(false); }

    private void End(bool success)
    {
        if (IsEnded) return;
        IsEnded = true;

        spawner.enabled = false;
        modeController.enabled = false;
        Time.timeScale = 0f;

        Debug.Log(success ? "임무 성공!" : "임무 실패");
        MissionEnded?.Invoke(success);
    }
}
