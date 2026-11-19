using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = 0.1f;
    [SerializeField] private float MaxSpeed = 6f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int movexHash = Animator.StringToHash("MoveX");
    private static readonly int movezHash = Animator.StringToHash("MoveZ");

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator를 찾지 못했습니다. Model에 Animator를 붙이세요.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Vector3 velocity = _rb.linearVelocity;
        velocity.y = 0f;
        
        Vector3 local = transform.InverseTransformDirection(velocity) / MaxSpeed;

        animator.SetFloat(movexHash, local.x, dampTime, Time.deltaTime);
        animator.SetFloat(movezHash, local.z, dampTime, Time.deltaTime);
    }
}