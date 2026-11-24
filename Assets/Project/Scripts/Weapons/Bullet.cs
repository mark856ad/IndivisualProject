using UnityEngine;

public enum BulletTeam { Player, Enemy, Neutral }

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifeTime = 3f;

    public float Damage { get; set; }
    public BulletTeam Owner { get; set; } = BulletTeam.Player;
    public SimplePool<Bullet> Pool { get; set; }

    private float spawnTime;

    private void OnEnable()
    {
        spawnTime = Time.time;
        Owner = BulletTeam.Player;
        Debug.Log($"[Bullet] 발사 방향 {transform.forward}, 회전 {transform.eulerAngles}");
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
        Debug.Log($"[Bullet] 닿음: {other.name} (트리거 {other.isTrigger}, 태그 {other.tag})");
        if (other.isTrigger) return;

        BulletTeam hitTeam = GetTeam(other);
        if (hitTeam == Owner) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(Damage);
        }

        Despawn();
    }

    private static BulletTeam GetTeam(Collider other)
    {
        if (other.CompareTag("Player")) return BulletTeam.Player;
        if (other.GetComponentInParent<Enemy>() != null) return BulletTeam.Enemy;
        return BulletTeam.Neutral;
    }

    private void Despawn()
    {
        if (Pool != null) Pool.Release(this);
        else Destroy(gameObject);
    }
}