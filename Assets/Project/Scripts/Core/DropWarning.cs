using UnityEngine;

public class DropWarning : MonoBehaviour
{
    [SerializeField] private Transform fill;

    private float startTime;
    private float duration;

    public static DropWarning Spawn(DropWarning prefab, Vector3 groundPosition, float radius, float seconds, float lift = 0.05f)
    {
        if (prefab == null || radius <= 0f) return null;

        DropWarning warning = Instantiate(prefab, groundPosition + Vector3.up * lift, Quaternion.identity);
        warning.Begin(radius, seconds);
        return warning;
    }

    private void Begin(float radius, float seconds)
    {
        transform.localScale = new Vector3(radius * 2f, 1f, radius * 2f);
        duration = Mathf.Max(0.01f, seconds);
        startTime = Time.time;
        SetFill(0f);
    }

    private void Update()
    {
        float t = Mathf.Clamp01((Time.time - startTime) / duration);
        SetFill(t);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private void SetFill(float t)
    {
        Vector3 scale = fill.localScale;
        scale.x = t;
        scale.z = t;
        fill.localScale = scale;
    }
}
