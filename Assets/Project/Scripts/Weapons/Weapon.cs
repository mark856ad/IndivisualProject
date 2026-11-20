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
    private bool ammoInitialized;
    private float reloadStartTime;
    protected SimplePool<Bullet> bulletPool;

    protected Quaternion AimRotation
    {
        get
        {
            Vector3 forward = muzzle.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.001f) forward = transform.forward;

            return Quaternion.LookRotation(forward.normalized);
        }
    }

    public event System.Action<Weapon> AmmoChanged;
    
    public string WeaponName => data.weaponName;
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => data.magazineSize;
    public int ReserveMagazines => reserveMagazines;
    public bool InfiniteReserve => data.infiniteReserve;
    public bool IsReloading => isReloading;
    public float ReloadTime => data.reloadTime;
    public float ReloadProgress =>
        isReloading ? Mathf.Clamp01((Time.time - reloadStartTime) / data.reloadTime) : 0f;
    public event System.Action Fired;

    private void NotifyAmmo()
    {
        Debug.Log($"[AmmoChanged] {WeaponName} {currentAmmo}/{MagazineSize}, 예비 {reserveMagazines}, 재장전 {isReloading}");
        AmmoChanged?.Invoke(this);
    }

    private void Awake()
    {
        InitAmmo();
        bulletPool = new SimplePool<Bullet>(bulletPrefab, 20, $"{data.weaponName} BulletPool");
    }
    private void OnEnable()
    {
        InitAmmo();
        NotifyAmmo();
    }
    private void OnDisable()
    {
        Debug.Log($"[장착] {data.weaponName}  탄 {currentAmmo}/{data.magazineSize}, 예비 {reserveMagazines}");
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
    private void InitAmmo()
    {
        if (ammoInitialized) return;
        ammoInitialized = true;

        currentAmmo = data.magazineSize;
        reserveMagazines = data.reserveMagazines;
    }
    public void AddSupply(int magazines)
    {
        if (data.infiniteReserve) return;

        InitAmmo();
        reserveMagazines = Mathf.Min(reserveMagazines + magazines, data.reserveMagazines);
        NotifyAmmo();
        Debug.Log($"{data.weaponName} 보급: 예비 탄창 {reserveMagazines}/{data.reserveMagazines}");
    }
    public void ResetAmmo()
    {
        ammoInitialized = true;
        isReloading = false;

        currentAmmo = data.magazineSize;
        reserveMagazines = data.reserveMagazines;
        NotifyAmmo();

        Debug.Log($"{data.weaponName} 새 탄창 지급: {currentAmmo}/{data.magazineSize}, 예비 {reserveMagazines}");
    }

    private void TryFire()
    {
        if (isReloading) return;
        if (currentAmmo <= 0) return;
        if (Time.time - lastFireTime < data.fireInterval) return;

        lastFireTime = Time.time;
        currentAmmo--;
        NotifyAmmo();

        Fire();
        Fired?.Invoke();
        SfxPlayer.Play(data.fireSound);
        Debug.Log($"탄: {currentAmmo}/{data.magazineSize}, 예비: {reserveMagazines}");
    }

    private void TryReload()
    {
        if (isReloading) return;
        if (reserveMagazines <= 0) return;
        if (currentAmmo >= data.magazineSize) return;
        if (!data.infiniteReserve && reserveMagazines <= 0) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        reloadStartTime = Time.time;
        NotifyAmmo();
        Debug.Log("재장전 중...");

        yield return new WaitForSeconds(data.reloadTime);

        currentAmmo = data.magazineSize;
        if (!data.infiniteReserve) reserveMagazines--;
        isReloading = false;
        NotifyAmmo();

        string reserveText = data.infiniteReserve ? "∞" : reserveMagazines.ToString();
        Debug.Log($"탄: {currentAmmo}/{data.magazineSize}, 예비: {reserveText}");
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