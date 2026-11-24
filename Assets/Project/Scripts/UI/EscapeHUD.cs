using TMPro;
using UnityEngine;

public class EscapeHUD : MonoBehaviour
{
    [SerializeField] private EscapeSequence escape;
    [SerializeField] private TMP_Text label;

    private void Update()
    {
        if (escape.IsCountingDown)
        {
            Show("SHUTTLE ARRIVING", escape.RemainingSeconds);
        }
        else if (escape.IsBoarding)
        {
            Show("BOARD NOW", escape.RemainingSeconds);
        }
        else
        {
            label.enabled = false;
        }
    }

    private void Show(string title, float remaining)
    {
        int seconds = Mathf.CeilToInt(remaining);

        label.enabled = true;
        label.text = $"{title} {seconds / 60:00}:{seconds % 60:00}";
    }
}