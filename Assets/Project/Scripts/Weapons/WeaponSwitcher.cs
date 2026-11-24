using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private Weapon[] primaryWeapons;
    [SerializeField] private int primaryIndex = 0;
    [SerializeField] private Weapon secondaryWeapon;
    [SerializeField] private Weapon supportWeapon;

    [SerializeField] private InputActionReference slot1Action;
    [SerializeField] private InputActionReference slot2Action;
    [SerializeField] private InputActionReference dropAction;

    private Weapon currentWeapon;
    private bool hasSupport;
    public event System.Action<Weapon> WeaponEquipped;

    private Weapon Slot1Weapon => hasSupport ? supportWeapon : primaryWeapons[primaryIndex];

    private void Start()
    {
        if (primaryWeapons == null || primaryWeapons.Length == 0)
        {
            Debug.LogError("Primary Weapons가 비어 있습니다!", this);
            return;
        }

        primaryIndex = Mathf.Clamp(primaryIndex, 0, primaryWeapons.Length - 1);
        Equip(Slot1Weapon);
    }

    private void Update()
    {
        if (slot1Action.action.WasPressedThisFrame()) Equip(Slot1Weapon);
        if (slot2Action.action.WasPressedThisFrame()) Equip(secondaryWeapon);
        if (dropAction.action.WasPressedThisFrame()) ReleaseSupport();
    }

    private void Equip(Weapon selected)
    {
        currentWeapon = selected;

        for (int i = 0; i < primaryWeapons.Length; i++)
        {
            primaryWeapons[i].gameObject.SetActive(primaryWeapons[i] == selected);
        }

        secondaryWeapon.gameObject.SetActive(secondaryWeapon == selected);

        if (supportWeapon != null)
        {
            supportWeapon.gameObject.SetActive(supportWeapon == selected);
        }
        WeaponEquipped?.Invoke(selected);
    }

    public void AcquireSupport()
    {
        if (supportWeapon == null) return;

        supportWeapon.ResetAmmo();
        hasSupport = true;
        Equip(supportWeapon);
        Debug.Log("지원무기 획득");
    }

    public void ReleaseSupport()
    {
        if (!hasSupport) return;

        bool wasHolding = currentWeapon == supportWeapon;
        hasSupport = false;

        if (wasHolding)
        {
            Equip(primaryWeapons[primaryIndex]);
        }
        else
        {
            supportWeapon.gameObject.SetActive(false);
        }

        Debug.Log($"지원무기 버림 (들고 있었음: {wasHolding})");
    }
}