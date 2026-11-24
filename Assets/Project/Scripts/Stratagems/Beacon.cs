using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Beacon : MonoBehaviour
{
    [Header("착지 판정")]
    [SerializeField] private float stopSpeed = 0.1f;
    [SerializeField] private float settleTime = 0.5f;
    [SerializeField] private float maxFlightTime = 6f;

    [Header("겉모습 (던지는 중 → 착지 후)")]
    [SerializeField] private GameObject closedModel;     // 던지는 동안 보이는 닫힌 비컨
    [SerializeField] private GameObject deployedModel;   // 착지하면 나타나는 열린 비컨
    [SerializeField] private float deployedYOffset = 0f; // 열린 비컨이 바닥에서 뜨거나 묻힐 때 조절

    [Header("빔 (다운받은 프리팹)")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private Vector3 beamEuler = new Vector3(-90f, 0f, 0f);
    [SerializeField] private float beamStartHeight = 0.3f;
    [SerializeField] private bool tintBeam = true;

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

        if (closedModel == null) Debug.LogError("Beacon의 Closed Model 칸이 비어 있습니다.", this);
        if (deployedModel == null) Debug.LogError("Beacon의 Deployed Model 칸이 비어 있습니다.", this);

        if (closedModel != null) closedModel.SetActive(true);
        if (deployedModel != null) deployedModel.SetActive(false);
    }

    public void Launch(StratagemData data, Vector3 velocity)
    {
        Data = data;
        HasLanded = false;
        isSlow = false;
        thrownTime = Time.time;

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

        SwapModels();
        SpawnBeam();

        Debug.Log($"{Data.stratagemName} 비컨 착지: {transform.position}");
        Landed?.Invoke(this);
    }

    private void SwapModels()
    {
        if (closedModel != null) closedModel.SetActive(false);
        if (deployedModel == null) return;

        // 열린 비컨은 기울어진 채로 서지 않게, 위를 향해 똑바로 세운다 (Y 회전만 유지)
        Vector3 position = transform.position + Vector3.up * deployedYOffset;
        Quaternion upright = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        deployedModel.transform.SetPositionAndRotation(position, upright);
        deployedModel.SetActive(true);
    }

    private void SpawnBeam()
    {
        if (beamPrefab == null) return;

        Vector3 position = transform.position + Vector3.up * beamStartHeight;
        GameObject beam = Instantiate(beamPrefab, position, Quaternion.Euler(beamEuler), transform);

        if (tintBeam && Data != null)
        {
            foreach (ParticleSystem ps in beam.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Data.beaconColor;
            }
        }
    }
}