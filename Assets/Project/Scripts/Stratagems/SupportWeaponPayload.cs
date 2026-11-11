using UnityEngine;

public class SupportWeaponPayload : StratagemPayload
{
    public override void Activate(Vector3 position)
    {
        Debug.Log("지원무기 포드 투하");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        WeaponSwitcher switcher = other.GetComponentInChildren<WeaponSwitcher>(true);
        if (switcher == null) return;

        switcher.AcquireSupport();
        Destroy(gameObject);
    }
}