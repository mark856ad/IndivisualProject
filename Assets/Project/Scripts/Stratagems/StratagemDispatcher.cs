using System.Collections;
using UnityEngine;

public class StratagemDispatcher : MonoBehaviour
{
    public void Dispatch(Beacon beacon)
    {
        StartCoroutine(DispatchRoutine(beacon));
    }

    private IEnumerator DispatchRoutine(Beacon beacon)
    {
        StratagemData data = beacon.Data;
        Vector3 position = beacon.transform.position;

        yield return new WaitForSeconds(data.callDelay);
        
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
}
