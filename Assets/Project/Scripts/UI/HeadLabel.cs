using TMPro;
using UnityEngine;

public class HeadLabel : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset;

    private TMP_Text text;
    private Camera cam;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        cam = Camera.main;

        if (text == null)
        {
            Debug.LogError("HeadLabel은 TextMeshPro 글자 오브젝트에 붙여야 합니다.", this);
            enabled = false;
            return;
        }

        if (followTarget == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) followTarget = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (!text.enabled || followTarget == null || cam == null) return;

        transform.SetPositionAndRotation(followTarget.position + offset, cam.transform.rotation);
    }
}