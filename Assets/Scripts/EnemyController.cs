using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int _damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        // IDamageable damageable = other.GetComponent<IDamageable>();
        // damageable?.TakeDamage(_damage);
        
        IDamageable damageable = ComponentRegistry.GetAs<IDamageable>(other.gameObject);
        damageable?.TakeDamage(_damage);
    }
}
