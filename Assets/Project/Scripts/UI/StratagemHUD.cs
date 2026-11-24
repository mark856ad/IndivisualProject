using System.Text;
using TMPro;
using UnityEngine;

public class StratagemHUD : MonoBehaviour
{
    [SerializeField] private StratagemInventory inventory;
    [SerializeField] private TMP_Text listText;
    [SerializeField] private float refreshInterval = 0.1f;
    [SerializeField] private StratagemInput stratagemInput;
    private readonly StringBuilder builder = new StringBuilder();
    private float lastRefreshTime;

    private void OnEnable()
    {
        inventory.StateChanged += OnStateChanged;
        stratagemInput.InputChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        inventory.StateChanged -= OnStateChanged;
        stratagemInput.InputChanged -= Refresh;
    }

    private void Update()
    {
        if (Time.unscaledTime - lastRefreshTime < refreshInterval) return;
        Refresh();
    }

    private void OnStateChanged(StratagemData data)
    {
        Refresh();
    }

    private void Refresh()
    {
        lastRefreshTime = Time.unscaledTime;
        builder.Clear();

        foreach (StratagemData data in inventory.Loadout)
        {
            builder.Append(data.stratagemName)
                .Append("  ")
                .Append(DirectionText.ToArrowsColored(data.code, GetMatchedCount(data)))
                .Append("   ")
                .Append(GetStatus(data))
                .Append('\n');
        }

        listText.text = builder.ToString();
    }

    private string GetStatus(StratagemData data)
    {
        if (data.maxUses > 0 && inventory.GetUsesRemaining(data) <= 0) return "USED";
        if (inventory.IsInFlight(data)) return "CALLING...";

        float cooldown = inventory.GetCooldownRemaining(data);
        if (cooldown > 0f) return $"CD {cooldown:F1}s";

        return data.maxUses > 0 ? $"x{inventory.GetUsesRemaining(data)}" : "READY";
    }
    private int GetMatchedCount(StratagemData data)
    {
        var input = stratagemInput.CurrentInput;
        if (input.Count == 0 || input.Count > data.code.Length) return 0;

        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] != data.code[i]) return 0;
        }

        return input.Count;
    }
}
