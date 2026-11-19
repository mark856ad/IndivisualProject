using UnityEngine;

public class WeaponLinkModer : MonoBehaviour
{
    [SerializeField] private GameObject model;

    private void OnEnable()
    {
        if (model != null) model.SetActive(true);
    }

    private void OnDisable()
    {
        if (model != null) model.SetActive(false);
    }
}