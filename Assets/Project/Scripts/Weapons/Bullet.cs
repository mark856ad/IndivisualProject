using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifeTime = 3f;

    public float Damage { get; set; }
    public SimplePool<Bullet> Pool { get; set; }

    private float spawnTime;

    private void OnEnable()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time - spawnTime >= lifeTime)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(Damage);
        }

        Despawn();
    }

    private void Despawn()
    {
        if (Pool != null) Pool.Release(this);
        else Destroy(gameObject);
    }
}