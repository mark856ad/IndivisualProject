using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimer : MonoBehaviour
{
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Camera mainCamera;

    public Vector3 AimPoint { get; private set; }

    private void Update()
    {
        Vector2 screenPos = lookAction.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            AimPoint = hit.point;
        }

        Vector3 direction = AimPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
