using UnityEngine;

[RequireComponent(typeof(ComponentProvider))]
public class PlayerController : MonoBehaviour, IDamageable
{
    private ComponentProvider _componentProvider;

    //private void Awake() => CacheComponents();
    
    private void CacheComponents()
    {
        _componentProvider = GetComponent<ComponentProvider>();
        
        _componentProvider.Add(this);
        _componentProvider.Add(GetComponent<Rigidbody>());
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"PlayerController : {damage} 데미지 입음");
    }
    

    public void Kill()
    {
        Debug.Log($"PlayerController : 데미지 입힘");
    }
}
