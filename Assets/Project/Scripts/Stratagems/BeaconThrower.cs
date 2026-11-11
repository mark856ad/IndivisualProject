using UnityEngine;
using UnityEngine.InputSystem;

public class BeaconThrower : MonoBehaviour
{
    [SerializeField] private InputModeController modeController;
    [SerializeField] private PlayerAimer aimer;
    [SerializeField] private StratagemInventory inventory;
    [SerializeField] private Beacon beaconPrefab;
    [SerializeField] private InputActionReference throwAction;
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private float flightTime = 0.8f;
    [SerializeField] private float maxThrowDistance = 15f;
    [SerializeField] private StratagemDispatcher dispatcher;

    private StratagemData held;

    public void Hold(StratagemData data)
    {
        held = data;
        modeController.SetMode(InputMode.Throw);
        Debug.Log($"{data.stratagemName} 비컨을 들었습니다. 좌클릭: 던지기 / 우클릭: 취소");
    }

    private void Update()
    {
        if (held == null) return;

        if (cancelAction.action.WasPressedThisFrame())
        {
            Cancel();
        }
        else if (throwAction.action.WasPressedThisFrame())
        {
            Throw();
        }
    }

    private void Cancel()
    {
        Debug.Log($"{held.stratagemName} 취소");
        held = null;
        modeController.SetMode(InputMode.Normal);
    }

    private void Throw()
    {
        StratagemData data = held;

        Vector3 spawnPos = transform.position + transform.forward * 1f + Vector3.up * 1.5f;
        Vector3 velocity = CalculateVelocity(spawnPos, aimer.AimPoint);

        Beacon beacon = Instantiate(beaconPrefab, spawnPos, Quaternion.identity);
        beacon.Launch(data, velocity);
        beacon.Landed += OnBeaconLanded;

        inventory.MarkInFlight(data);

        held = null;
        modeController.SetMode(InputMode.Normal);
    }

    private Vector3 CalculateVelocity(Vector3 from, Vector3 target)
    {
        Vector3 flat = target - from;
        flat.y = 0f;

        float distance = Mathf.Min(flat.magnitude, maxThrowDistance);
        Vector3 direction = flat.sqrMagnitude > 0.001f ? flat.normalized : transform.forward;

        float gravity = -Physics.gravity.y;
        float verticalSpeed = (target.y - from.y + 0.5f * gravity * flightTime * flightTime) / flightTime;

        return direction * (distance / flightTime) + Vector3.up * verticalSpeed;
    }

    private void OnBeaconLanded(Beacon beacon)
    {
        beacon.Landed -= OnBeaconLanded;
        inventory.Consume(beacon.Data);
        dispatcher.Dispatch(beacon);
    }
}