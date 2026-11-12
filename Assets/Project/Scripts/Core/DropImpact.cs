using System.Collections.Generic;
using UnityEngine;

public static class DropImpact
{
    public static int Strike(Vector3 position, float radius)
    {
        if (radius <= 0f) return 0;

        Collider[] hits = Physics.OverlapSphere(position, radius);
        HashSet<IDamageable> killed = new HashSet<IDamageable>();

        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target == null) continue;
            if (!killed.Add(target)) continue;

            target.Kill();
        }

        return killed.Count;
    }
}
