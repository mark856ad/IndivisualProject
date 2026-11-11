using UnityEngine;

public class ResupplyPayload : StratagemPayload
{
    [SerializeField] private int magazines = 2;
    [SerializeField] private float healAmount = 30f;

    public override void Activate(Vector3 position)
    {
        Debug.Log("보급 상자 투하");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (Weapon weapon in other.GetComponentsInChildren<Weapon>(true))
        {
            weapon.AddSupply(magazines);
        }

        if (other.TryGetComponent(out PlayerHealth health))
        {
            health.Heal(healAmount);
        }

        Destroy(gameObject);
    }
}