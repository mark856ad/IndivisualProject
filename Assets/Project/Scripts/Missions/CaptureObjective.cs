using UnityEngine;

public class CaptureObjective : Objective
{
    [SerializeField] private float radius = 6f;
    [SerializeField] private float gainRate = 0.1f;
    [SerializeField] private float contestedRate = 0.02f;
    [SerializeField] private float decayRate = 0.3f;

    private float progress;

    public bool IsContested { get; private set; }
    public bool PlayerInside { get; private set; }

    public override string Title => "CAPTURE THE AREA";
    public override float Progress => progress;
    private bool lastPlayerInside;
    private void Update()
    {
        if (IsCompleted) return;

        ScanZone();
        if (PlayerInside != lastPlayerInside)
        {
            lastPlayerInside = PlayerInside;
            Debug.Log($"구역 {(PlayerInside ? "진입" : "이탈")}, 진행도 {progress:P0}");
        }
        
        float rate;
        if (!PlayerInside) rate = -decayRate;
        else if (IsContested) rate = contestedRate;
        else rate = gainRate;

        float next = Mathf.Clamp01(progress + rate * Time.deltaTime);
        if (Mathf.Approximately(next, progress)) return;

        progress = next;
        NotifyProgress();
        Debug.Log($"점령 {progress:P0} (적 있음: {IsContested})");
        if (progress >= 1f)
        {
            Complete();
        }
    }

    private void ScanZone()
    {
        PlayerInside = false;
        IsContested = false;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player")) PlayerInside = true;
            else if (hit.GetComponentInParent<Enemy>() != null) IsContested = true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
