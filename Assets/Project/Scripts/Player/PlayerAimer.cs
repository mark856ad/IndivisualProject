using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimer : MonoBehaviour
{
    [SerializeField] private InputActionReference _aimAction;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Camera _mainCamera;
    
    public Vector3 _aimPoint {get; private set;}

    private void OnEnable()
    {
        _aimAction.action.Enable();
    }
    
    private void OnDisable()
    {
        _aimAction.action.Disable();
    }

    private void Update()
    {
        Vector2 _screenPoint = _aimAction.action.ReadValue<Vector2>();
        Ray ray = _mainCamera.ScreenPointToRay(_screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundLayer))
        {
            _aimPoint = hit.point;
        }

        Vector3 direction = _aimPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);    
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_aimPoint, 0.3f);
    }
}
