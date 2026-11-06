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

    private void Awake()
    {
        currentAmmo = data.magazineSize;
        reserveMagazines = data.reserveMagazines;
    }

    private void OnEnable()
    {
        fireAction.action.Enable();
        reloadAction.action.Enable();
    }

    private void OnDisable()
    {
        fireAction.action.Disable();
        reloadAction.action.Disable();
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
    }
    protected abstract void Fire();

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
}