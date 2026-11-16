using TMPro;
using UnityEngine;

public class ReloadLabelHUD : MonoBehaviour
{
    [SerializeField] private WeaponSwitcher switcher;
    [SerializeField] private TMP_Text label;

    private Weapon current;

    private void Awake()
    {
        label.enabled = false;
    }

    private void OnEnable()
    {
        switcher.WeaponEquipped += OnWeaponEquipped;
    }

    private void OnDisable()
    {
        switcher.WeaponEquipped -= OnWeaponEquipped;

        if (current != null)
        {
            current.AmmoChanged -= Refresh;
            current = null;
        }
    }

    private void OnWeaponEquipped(Weapon weapon)
    {
        if (current != null)
        {
            current.AmmoChanged -= Refresh;
        }

        current = weapon;
        current.AmmoChanged += Refresh;
        Refresh(current);
    }

    private void Refresh(Weapon weapon)
    {
        bool reloading = weapon.IsReloading;
        label.enabled = reloading;

        if (reloading)
        {
            label.text = "Reloading!";
        }
    }
}