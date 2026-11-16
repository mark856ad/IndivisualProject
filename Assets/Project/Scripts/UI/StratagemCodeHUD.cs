using System.Text;
using TMPro;
using UnityEngine;

public class StratagemCodeHUD : MonoBehaviour
{
    [SerializeField] private StratagemInput stratagemInput;
    [SerializeField] private StratagemInventory inventory;
    [SerializeField] private TMP_Text label;

    private readonly StringBuilder builder = new StringBuilder();

    private void OnEnable()
    {
        stratagemInput.InputChanged += Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        stratagemInput.InputChanged -= Refresh;
    }

    private void Update()
    {
        if (label.enabled && !stratagemInput.enabled)
        {
            label.enabled = false;
        }
    }

    private void Refresh()
    {
        var input = stratagemInput.CurrentInput;
        builder.Clear();

        if (input.Count > 0)
        {
            foreach (StratagemData data in inventory.Loadout)
            {
                int matched = DirectionText.MatchedPrefixCount(input, data.code);
                if (matched == 0) continue;

                builder.Append(data.stratagemName)
                    .Append("  ")
                    .Append(DirectionText.ToArrowsColored(data.code, matched))
                    .Append('\n');
            }
        }

        bool show = builder.Length > 0;
        label.enabled = show;

        if (show)
        {
            label.text = builder.ToString();
        }
    }
}