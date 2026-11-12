using TMPro;
using UnityEngine;

public class EscapeCodeHUD : MonoBehaviour
{
    [SerializeField] private EscapeSequence escape;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.2f, 0f);

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

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

    private void LateUpdate()
    {
        if (!label.enabled) return;

        label.transform.position = followTarget.position + offset;
        label.transform.rotation = cam.transform.rotation;
    }

    private void Refresh()
    {
        bool show = escape.IsWaitingForCode && escape.PlayerInRange && escape.Code != null;
        label.enabled = show;

        if (!show) return;

        label.text = "CALL SHUTTLE\n" + DirectionText.ToArrowsColored(escape.Code, escape.MatchedCount);
    }
}