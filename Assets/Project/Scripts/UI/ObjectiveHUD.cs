using System.Text;
using TMPro;
using UnityEngine;

public class ObjectiveHUD : MonoBehaviour
{
    [SerializeField] private ObjectiveManager manager;
    [SerializeField] private TMP_Text listText;

    private readonly StringBuilder builder = new StringBuilder();

    private void OnEnable()
    {
        foreach (Objective objective in manager.Objectives)
        {
            objective.ProgressChanged += OnObjectiveChanged;
            objective.Completed += OnObjectiveChanged;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        foreach (Objective objective in manager.Objectives)
        {
            objective.ProgressChanged -= OnObjectiveChanged;
            objective.Completed -= OnObjectiveChanged;
        }
    }

    private void OnObjectiveChanged(Objective objective)
    {
        Refresh();
    }

    private void Refresh()
    {
        builder.Clear();

        foreach (Objective objective in manager.Objectives)
        {
            builder.Append(objective.IsCompleted ? "[x] " : "[ ] ")
                .Append(objective.Title)
                .Append("   ")
                .Append(objective.ProgressText)
                .Append('\n');

            if (!string.IsNullOrEmpty(objective.Hint))
            {
                builder.Append("    <color=#FFD94A>").Append(objective.Hint).Append("</color>\n");
            }
        }

        listText.text = builder.ToString();
    }
}