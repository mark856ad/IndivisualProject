using System;
using System.Collections.Generic;
using UnityEngine;

public class StratagemInventory : MonoBehaviour
{
    [SerializeField] private StratagemData[] loadout;

    public StratagemData[] Loadout => loadout;
    public event Action<StratagemData> StateChanged;

    private readonly Dictionary<StratagemData, StratagemState> states
        = new Dictionary<StratagemData, StratagemState>();

    private void Awake()
    {
        foreach (StratagemData data in loadout)
        {
            if (states.ContainsKey(data))
            {
                Debug.LogError($"로드아웃에 '{data.stratagemName}'이 중복되어 있습니다.", this);
                continue;
            }

            states.Add(data, new StratagemState { remainingUses = data.maxUses, readyTime = 0f });
        }
    }

    public float GetCooldownRemaining(StratagemData data)
    {
        if (!states.TryGetValue(data, out StratagemState state)) return 0f;
        return Mathf.Max(0f, state.readyTime - Time.time);
    }

    public int GetUsesRemaining(StratagemData data)
    {
        return states.TryGetValue(data, out StratagemState state) ? state.remainingUses : 0;
    }

    public void Consume(StratagemData data)
    {
        if (!states.TryGetValue(data, out StratagemState state)) return;

        state.isInFlight = false;   // 추가: 착지했으니 비행 상태 해제

        if (data.maxUses > 0) state.remainingUses--;
        state.readyTime = Time.time + data.cooldown;

        StateChanged?.Invoke(data);
    }
    
    public bool CanUse(StratagemData data)
    {
        if (!states.TryGetValue(data, out StratagemState state)) return false;
        if (state.isInFlight) return false;
        if (data.maxUses > 0 && state.remainingUses <= 0) return false;

        return Time.time >= state.readyTime;
    }

    public bool IsInFlight(StratagemData data)
    {
        return states.TryGetValue(data, out StratagemState state) && state.isInFlight;
    }

    public void MarkInFlight(StratagemData data)
    {
        if (!states.TryGetValue(data, out StratagemState state)) return;
        state.isInFlight = true;
    }
}