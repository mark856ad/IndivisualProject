using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private WeaponSwitcher switcher;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image reloadIcon;

    private Weapon current;
    
    private void Update()
    {
        bool reloading = current != null && current.IsReloading;

        reloadIcon.enabled = reloading;

        if (reloading)
        {
            reloadIcon.fillAmount = 1f - current.ReloadProgress;
        }
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
        weaponNameText.text = weapon.WeaponName;

        string reserve = weapon.InfiniteReserve ? "∞" : weapon.ReserveMagazines.ToString();
        string reloading = weapon.IsReloading ? "  Reloading!" : "";

        ammoText.text = $"{weapon.CurrentAmmo}/{weapon.MagazineSize}  x{reserve}{reloading}";
    }
}