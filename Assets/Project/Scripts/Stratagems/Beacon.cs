using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Beacon : MonoBehaviour
{
    [SerializeField] private float stopSpeed = 0.1f;
    [SerializeField] private float settleTime = 0.5f;
    [SerializeField] private float maxFlightTime = 6f;
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private GameObject beam;

    public StratagemData Data { get; private set; }
    public bool HasLanded { get; private set; }
    public event Action<Beacon> Landed;

    private Rigidbody rb;
    private float thrownTime;
    private float slowStartTime;
    private bool isSlow;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        beam.SetActive(false);
    }

    public void Launch(StratagemData data, Vector3 velocity)
    {
        Data = data;
        HasLanded = false;
        isSlow = false;
        thrownTime = Time.time;

        bodyRenderer.material.color = data.beaconColor;
        rb.linearVelocity = velocity;
    }

    private void FixedUpdate()
    {
        if (HasLanded) return;

        if (Time.time - thrownTime >= maxFlightTime)
        {
            Land();
            return;
        }

        bool slow = rb.linearVelocity.sqrMagnitude < stopSpeed * stopSpeed;

        if (!slow)
        {
            isSlow = false;
            return;
        }

        if (!isSlow)
        {
            isSlow = true;
            slowStartTime = Time.time;
            return;
        }

        if (Time.time - slowStartTime >= settleTime)
        {
            Land();
        }
    }

    private void Land()
    {
        HasLanded = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        beam.transform.SetPositionAndRotation(transform.position + Vector3.up * 10f, Quaternion.identity);
        beam.SetActive(true);

        Debug.Log($"{Data.stratagemName} 비컨 착지: {transform.position}");
        Landed?.Invoke(this);
    }
}