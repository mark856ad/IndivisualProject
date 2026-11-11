using UnityEngine;

public abstract class StratagemPayload : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    public float Lifetime => lifetime;

    public abstract void Activate(Vector3 position);
}
