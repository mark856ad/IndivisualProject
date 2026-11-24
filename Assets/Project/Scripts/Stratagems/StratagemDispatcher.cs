using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class StratagemDispatcher : MonoBehaviour
{
    [SerializeField] private DropWarning warningPrefab;

    public void Dispatch(Beacon beacon)
    {
        StartCoroutine(DispatchRoutine(beacon));
    }

    private IEnumerator DispatchRoutine(Beacon beacon)
    {
        StratagemData data = beacon.Data;
        Vector3 position = beacon.transform.position;

        ShowWarnings(data, position);
        PendingCall call = new PendingCall
        {
            Name = data.stratagemName,
            ArrivalTime = Time.time + data.callDelay
        };
        pending.Add(call);

        yield return new WaitForSeconds(data.callDelay);
        pending.Remove(call);

        int killed = DropImpact.Strike(position, data.impactRadius);
        if (killed > 0)
        {
            Debug.Log($"{data.stratagemName} 투하 충격: {killed}명 사망 (아군/적군 구분 없음)");
        }

        if (data.payloadPrefab != null)
        {
            StratagemPayload payload = Instantiate(data.payloadPrefab, position, Quaternion.identity);
            payload.Activate(position);
            Destroy(payload.gameObject, payload.Lifetime);
        }

        Destroy(beacon.gameObject);
    }

    private void ShowWarnings(StratagemData data, Vector3 position)
    {
        Vector3 ground = position - Vector3.up * 0.2f;

        float danger = data.payloadPrefab != null ? data.payloadPrefab.DangerRadius : 0f;
        DropWarning.Spawn(warningPrefab, ground, danger, data.callDelay, 0.05f);

        if (!Mathf.Approximately(data.impactRadius, danger))
        {
            DropWarning.Spawn(warningPrefab, ground, data.impactRadius, data.callDelay, 0.12f);
        }
    }
    public class PendingCall
    {
        public string Name;
        public float ArrivalTime;
    }

    private readonly List<PendingCall> pending = new List<PendingCall>();

    public IReadOnlyList<PendingCall> Pending => pending;
}
