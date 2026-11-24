using System.Text;
using TMPro;
using UnityEngine;

public class SupportTimerHUD : MonoBehaviour
{
    [SerializeField] private StratagemDispatcher dispatcher;
    [SerializeField] private TMP_Text label;

    private readonly StringBuilder builder = new StringBuilder();

    private void Update()
    {
        var pending = dispatcher.Pending;

        if (pending.Count == 0)
        {
            label.enabled = false;
            return;
        }

        builder.Clear();

        foreach (StratagemDispatcher.PendingCall call in pending)
        {
            float remaining = Mathf.Max(0f, call.ArrivalTime - Time.time);
            builder.Append(call.Name)
                .Append("  ")
                .Append(remaining.ToString("F1"))
                .Append("s\n");
        }

        label.enabled = true;
        label.text = "SUPPORT INBOUND\n" + builder;
    }
}
