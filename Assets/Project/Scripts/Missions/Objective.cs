using System;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public abstract string Title { get; }
    public abstract float Progress { get; }   // 0 ~ 1

    public bool IsCompleted { get; private set; }

    public event Action<Objective> ProgressChanged;
    public event Action<Objective> Completed;

    protected void NotifyProgress()
    {
        ProgressChanged?.Invoke(this);
    }

    protected void Complete()
    {
        if (IsCompleted) return;

        IsCompleted = true;
        Completed?.Invoke(this);
    }
}