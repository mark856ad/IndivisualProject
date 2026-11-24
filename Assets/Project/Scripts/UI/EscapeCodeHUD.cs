using TMPro;
using UnityEngine;

public class EscapeCodeHUD : MonoBehaviour
{
    [SerializeField] private EscapeSequence escape;
    [SerializeField] private TMP_Text label;

    private void OnEnable()
    {
        escape.InputChanged += Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        escape.InputChanged -= Refresh;
    }

    private void Refresh()
    {
        bool show = escape.IsWaitingForCode && escape.PlayerInRange && escape.Code != null;
        label.enabled = show;

        if (!show) return;

        label.text = "CALL SHUTTLE\n" + DirectionText.ToArrowsColored(escape.Code, escape.MatchedCount);
    }
}