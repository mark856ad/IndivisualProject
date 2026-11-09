using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private Weapon[] primaryWeapons;
    [SerializeField] private int primaryIndex = 0;
    [SerializeField] private Weapon secondaryWeapon;
    [SerializeField] private InputActionReference slot1Action;
    [SerializeField] private InputActionReference slot2Action;

    private void Start()
    {
        primaryIndex = Mathf.Clamp(primaryIndex, 0, primaryWeapons.Length - 1);
        Equip(primaryWeapons[primaryIndex]);
    }

    private void Update()
    {
        if (slot1Action.action.WasPressedThisFrame()) Equip(primaryWeapons[primaryIndex]);
        if (slot2Action.action.WasPressedThisFrame()) Equip(secondaryWeapon);
    }

    private void Equip(Weapon selected)
    {
        for (int i = 0; i < primaryWeapons.Length; i++)
        {
            primaryWeapons[i].gameObject.SetActive(primaryWeapons[i] == selected);
        }

        secondaryWeapon.gameObject.SetActive(secondaryWeapon == selected);
    }
}