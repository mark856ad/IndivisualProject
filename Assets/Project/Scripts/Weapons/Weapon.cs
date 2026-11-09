using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData data;
    [SerializeField] protected Bullet bulletPrefab;
    [SerializeField] protected Transform muzzle;
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference reloadAction;

    private float lastFireTime = -999f;
    private int currentAmmo;
    private int reserveMagazines;
    private bool isReloading;
    protected SimplePool<Bullet> bulletPool;

    private void Awake()
    {
        currentAmmo = data.magazineSize;
        reserveMagazines = data.reserveMagazines;
        bulletPool = new SimplePool<Bullet>(bulletPrefab, 20, $"{data.weaponName} BulletPool");
    }
    private void OnDisable()
    {
s        Debug.Log($"[장착] {data.weaponName}  탄 {currentAmmo}/{data.magazineSize}, 예비 {reserveMagazines}");
        isReloading = false;   // 재장전 취소 (코루틴은 꺼질 때 자동 중단)
    }

    private void Update()
    {
        if (reloadAction.action.WasPressedThisFrame())
        {
            TryReload();
        }

        if (fireAction.action.IsPressed())
        {
            TryFire();
        }
    }

    private void TryFire()
    {
        if (isReloading) return;
        if (currentAmmo <= 0) return;
        if (Time.time - lastFireTime < data.fireInterval) return;

        lastFireTime = Time.time;
        currentAmmo--;

        Fire();

        Debug.Log($"탄: {currentAmmo}/{data.magazineSize}, 예비: {reserveMagazines}");
    }

    private void TryReload()
    {
        if (isReloading) return;
        if (reserveMagazines <= 0) return;
        if (currentAmmo >= data.magazineSize) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("재장전 중...");

        yield return new WaitForSeconds(data.reloadTime);

        currentAmmo = data.magazineSize;
        reserveMagazines--;
        isReloading = false;

        Debug.Log($"재장전 완료. 예비: {reserveMagazines}");
    }

    protected abstract void Fire();
    protected Bullet SpawnBullet(Quaternion rotation)
    {
        Bullet bullet = bulletPool.Get(muzzle.position, rotation);
        bullet.Pool = bulletPool;
        bullet.Damage = data.damage;
        return bullet;
    }
}