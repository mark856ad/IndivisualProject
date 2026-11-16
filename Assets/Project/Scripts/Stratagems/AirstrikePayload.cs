using UnityEngine;

public class AirstrikePayload : StratagemPayload
{
    [SerializeField] private float radius = 8f;
    [SerializeField] private float damage = 100f;
    [SerializeField] private LayerMask damageMask = ~0;
    
    public override float DangerRadius => radius;
    public override void Activate(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, damageMask);

        foreach (Collider hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable target)) continue;

            Vector3 offset = hit.transform.position - position;
            offset.y = 0f;

            float ratio = 1f - Mathf.Clamp01(offset.magnitude / radius);
            float finalDamage = damage * ratio;

            target.TakeDamage(finalDamage);
            Debug.Log($"폭격 피해: {hit.name} 거리 {offset.magnitude:F1}, 피해 {finalDamage:F0}");
        }
    }
}
